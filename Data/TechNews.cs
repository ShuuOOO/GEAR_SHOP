using System;
using System.Collections.Generic;

namespace TL4_SHOP.Data;

public partial class TechNews
{
    public int TechNewsId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Summary { get; set; }

    public string ContentHtml { get; set; } = null!;

    public string? CoverImage { get; set; }

    public string? Author { get; set; }

    public string? Tags { get; set; }

    public DateTime PublishedAt { get; set; }

    public bool IsFeatured { get; set; }

    public int ViewCount { get; set; }
}
