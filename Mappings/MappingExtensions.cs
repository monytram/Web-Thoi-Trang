﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace WebThoiTrang.Mappings
{
    public static class MappingExtensions
    {
        public static async Task<List<TDestination>> ProjectToListAsync<TDestination>(
            this IQueryable source, AutoMapper.IConfigurationProvider configuration)
        {
            return await source.ProjectTo<TDestination>((AutoMapper.IConfigurationProvider)configuration).ToListAsync();
        }

        public static PaginatedList<TDestination> ToPaginatedList<TDestination>(
            this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
        {
            return PaginatedList<TDestination>.Create(queryable, pageNumber, pageSize);
        }

        public static async Task<PaginatedList<TDestination>> ToPaginatedListAsync<TSource, TDestination>(
            this IQueryable<TSource> query,
            IMapper mapper,
            int pageNumber,
            int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TDestination>(mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedList<TDestination>(items, count, pageNumber, pageSize);
        }
    }

    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
