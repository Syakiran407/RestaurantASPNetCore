using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using J72443Restaurant.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace J72443Restaurant.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _UserManager;
        public IList<CheckoutItem> Items { get; private set; }
        public OrderHistory Order = new OrderHistory();

        public decimal Total = 0;
        public long AmountPayable = 0;


        public CheckoutModel(AppDbContext db, UserManager<ApplicationUser> UserManager)
        {
            _db = db;
            _UserManager = UserManager;
        }

        public async Task OnGetAsync()
        {
            var user = await _UserManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email);

            Items = _db.CheckoutItems.FromSql(
                "SELECT Foods.ID, Foods.Price, " +
                "Foods.FoodName, " +
                "BasketItems.BasketID, BasketItems.Quantity " +
                "FROM Foods INNER JOIN BasketItems " +
                "ON Foods.ID = BasketItems.StockID " +
                "WHERE BasketID = {0}", customer.BasketID
                ).ToList();

            Total = 0; 
            foreach (var item in Items)
            {
                Total = Total + (item.Quantity * item.Price);
            }

            AmountPayable = (long)(Total * 100);

        }

        public async Task<IActionResult> OnPostBuyAsync(int itemID)
        {

        
            var currentOrder = _db.OrderHistories.FromSql("SELECT * From OrderHistories").OrderByDescending(b => b.OrderNo).FirstOrDefault();

            if (currentOrder == null)
            {
                Order.OrderNo = 1;
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1;
            }

            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email;
            _db.OrderHistories.Add(Order);

            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(user.Email);

            var basketItems =
                _db.BasketItems
                .FromSql("SELECT * From BasketItems " +
                "WHERE BasketID = {0}", customer.BasketID)
                .ToList();

            Total = 0;

            foreach (var item in basketItems)
            {
                Data.OrderItem oi = new Data.OrderItem
                {
                    OrderNo = Order.OrderNo,
                    StockID = item.StockID,
                    Quantity = item.Quantity
                };

                _db.OrderItems.Add(oi);   

            } 

            await _db.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostChargeAsync(string stripeEmail, string stripeToken, long amount)
        {
            var currentOrder = _db.OrderHistories.FromSql("SELECT * From OrderHistories").OrderByDescending(b => b.OrderNo).FirstOrDefault();

            if (currentOrder == null)
            {
                Order.OrderNo = 1;
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1;
            }

            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email;
            _db.OrderHistories.Add(Order);

            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(user.Email);

            var basketItems =
                _db.BasketItems
                .FromSql("SELECT * From BasketItems " +
                "WHERE BasketID = {0}", customer.BasketID)
                .ToList();


            foreach (var item in basketItems)
            {
                Data.OrderItem oi = new Data.OrderItem
                {
                    OrderNo = Order.OrderNo,
                    StockID = item.StockID,
                    Quantity = item.Quantity
                };

                _db.OrderItems.Add(oi);
                _db.BasketItems.Remove(item);

            }
                await _db.SaveChangesAsync();

                var customers = new CustomerService();
                var charges = new ChargeService();

                var customer_ = customers.Create(new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    Source = stripeToken
                });

                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = amount,
                    Description = "J72443 Restaurant Charge",
                    Currency = "gbp",
                    CustomerId = customer_.Id
                });
            
            return RedirectToPage("/Account/Success");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int itemID)
        {
            var user = await _UserManager.GetUserAsync(User);
            Order.Email = user.Email;

            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(user.Email);

                _db.BasketItems
                .FromSql("SELECT * From BasketItems " +
                "WHERE BasketID = {0}", customer.BasketID)
                .ToList();

            var item = await _db.BasketItems.FindAsync(itemID, customer.BasketID);
            if (item != null)
            {
                _db.BasketItems.Remove(item);
            
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnUpdateAsync(int itemID)
        {
            var user = await _UserManager.GetUserAsync(User);

            CheckoutCustomer customer = await _db
            .CheckoutCustomers
            .FindAsync(user.Email);

            var item = _db.BasketItems.FromSql("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", itemID, customer.BasketID)
            .ToList()
            .FirstOrDefault();

            if (item == null)
            {
                BasketItem newItem = new BasketItem
                {
                    BasketID = customer.BasketID,
                    StockID = itemID,
                    Quantity = 1
                };
                _db.BasketItems.Add(newItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                item.Quantity = item.Quantity + 1;
                _db.Attach(item).State = EntityState.Modified;
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw new Exception($"Basket not found!", e);
                }
            }
            return RedirectToPage("/Checkout");
        }
    }

}