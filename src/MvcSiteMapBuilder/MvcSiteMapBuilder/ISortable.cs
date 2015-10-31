﻿namespace MvcSiteMapBuilder
{
    /// <summary>
    /// Contract to specify a sortable entity.
    /// </summary>
    public interface ISortable
    {
        int Order { get; }
    }
}
