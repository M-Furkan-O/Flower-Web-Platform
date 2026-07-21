using FlowerShop.API.Data;
using FlowerShop.API.DTOs;
using FlowerShop.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var district = await _context.Districts.FindAsync(dto.DistrictId);
            if (district == null)
                return BadRequest("Geçersiz teslimat ilçesi seçildi.");

            decimal subTotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"ID'si {item.ProductId} olan ürün bulunamadı.");

                subTotal += product.Price * item.Quantity;
                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                Address = dto.Address,
                DistrictId = dto.DistrictId,
                SubTotal = subTotal,
                DeliveryFee = district.BaseDeliveryFee,
                GrandTotal = subTotal + district.BaseDeliveryFee,
                OrderStatus = "Alındı",
                OrderDate = DateTime.UtcNow,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Sipariş başarıyla oluşturuldu!",
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                DistrictName = district.Name,
                SubTotal = order.SubTotal,
                DeliveryFee = order.DeliveryFee,
                GrandTotal = order.GrandTotal,
                OrderStatus = order.OrderStatus,
                OrderDate = order.OrderDate
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Select(o => new
                {
                    o.Id,
                    o.CustomerName,
                    o.CustomerPhone,
                    o.Address,
                    DistrictName = o.District.Name,
                    o.SubTotal,
                    o.DeliveryFee,
                    o.GrandTotal,
                    o.OrderStatus,
                    o.OrderDate,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.ProductId,
                        ProductName = i.Product.Name,
                        i.Quantity,
                        i.UnitPrice
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}