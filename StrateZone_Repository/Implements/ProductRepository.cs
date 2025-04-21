using MealHunt_Repositories.Pagination;
using Microsoft.EntityFrameworkCore;
using StrateZone_Repository.Data;
using StrateZone_Repository.Entities;
using StrateZone_Repository.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrateZone_Repository.Implements
{
    public class ProductRepository
    {
        private readonly StrateZoneDbContext _context;

        public ProductRepository( StrateZoneDbContext context )
        {
            _context = context;
        }

        public async Task<PagedList<Product>> GetProductsAsync(TablesAppointmentParameters parameters, string? searchTerm)
        {
            try
            {
                var result = _context.Products.AsNoTracking()
                                            .Where(p => searchTerm == null || p.ProductName.ToLower().Contains(searchTerm.ToLower()))
                                            .AsQueryable();

                return await PagedList<Product>.ToPagedList(result, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex) 
            {
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Products
                                .AsNoTracking()
                                .SingleOrDefaultAsync(p => p.ProductId == id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            try
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return product; 
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Product>> CreateProductsAsync(List<Product> products)
        {
            try
            {
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
