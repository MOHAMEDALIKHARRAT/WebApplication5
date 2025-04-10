﻿using Microsoft.EntityFrameworkCore;
using WebApplication5.Data;
using WebApplication5.Models;

namespace WebApplication5.Repository // Note: Namespace corrected to match your provided code
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SaleRepository> _logger; // Added for logging

        public SaleRepository(AppDbContext context, ILogger<SaleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all sales");
                var sales = await _context.Sales
                    .Include(s => s.Client) // Include the navigation property 'Client'
                    .ToListAsync();
                _logger.LogInformation($"Retrieved {sales.Count} sales");
                return sales;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all sales");
                throw;
            }
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Retrieving sale with ID: {id}");
                var sale = await _context.Sales
                    .Include(s => s.Client) // Include the navigation property 'Client'
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (sale == null)
                {
                    _logger.LogWarning($"Sale with ID: {id} not found");
                }
                return sale;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving sale with ID: {id}");
                throw;
            }
        }

        public async Task<Sale> AddAsync(Sale sale)
        {
            try
            {
                _logger.LogInformation($"Adding sale with DocRef: {sale.DocRef}");
                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Added sale with ID: {sale.Id}");
                return sale;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding sale with DocRef: {sale.DocRef}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Sale sale)
        {
            try
            {
                _logger.LogInformation($"Updating sale with ID: {sale.Id}");
                var existing = await _context.Sales.FindAsync(sale.Id);
                if (existing == null)
                {
                    _logger.LogWarning($"Sale with ID: {sale.Id} not found for update");
                    return false;
                }

                existing.DocRef = sale.DocRef;
                existing.TiersId = sale.TiersId;
                existing.DocRepresentant = sale.DocRepresentant;
                existing.DocNetAPayer = sale.DocNetAPayer;
                existing.DocDate = sale.DocDate;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Updated sale with ID: {sale.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating sale with ID: {sale.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting sale with ID: {id}");
                var sale = await _context.Sales.FindAsync(id);
                if (sale == null)
                {
                    _logger.LogWarning($"Sale with ID: {id} not found for deletion");
                    return false;
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Deleted sale with ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting sale with ID: {id}");
                throw;
            }
        }
    }
}