﻿using System.Linq.Expressions;
using Apiand.Extensions.DDD;
using XXXnameXXX.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace XXXnameXXX.Infrastructure.Contracts;

public class EfCoreRepository<T>(DbContext context) : IRepository<T>
    where T: Entity
{
    public async Task<T?> GetByIdAsync(string id)
    {
        var element = await context.Set<T>().FindAsync(id);
        return element;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var elements = await context.Set<T>().ToListAsync();
        return elements;
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        var elements = await context.Set<T>().Where(filter).ToListAsync();
        return elements;
    }

    public async Task AddAsync(T entity)
    {
        await context.Set<T>().AddAsync(entity);
    }

    public Task UpdateAsync(string id, T entity)
    {
        context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(string id)
    {
        var element = await GetByIdAsync(id);
        if (element is not null) 
            context.Set<T>().Remove(element);
    }
}