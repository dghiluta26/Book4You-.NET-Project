using Project.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Project.Services;

public class PdfService : IPdfService
{
    // Project brand color (#385144 dark forest green, taken from navbar)
    private const string ColorPrimary = "#385144";
    private const string ColorPrimaryLight = "#f0f4f1";
    private const string ColorBorder = "#d8e0da";
    private const string ColorTextMuted = "#666666";
    private const string ColorWhite = "#ffffff";

    private readonly IWebHostEnvironment _env;

    public PdfService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string? GenerateBookingPdf(Booking booking)
    {
        var dir = Path.Combine(_env.WebRootPath, "pdfs", "bookings");
        Directory.CreateDirectory(dir);

        var fileName = $"booking_{booking.Id}.pdf";
        var filePath = Path.Combine(dir, fileName);
        var relativeUrl = $"/pdfs/bookings/{fileName}";

        // Idempotent — skip generation if the file already exists
        if (File.Exists(filePath))
            return relativeUrl;

        var nights = Math.Max(1, (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days);
        var pricePerNight = booking.TotalPrice / nights;

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(44);
                page.MarginVertical(36);
                page.DefaultTextStyle(t => t.FontSize(10).FontColor("#333333").FontFamily(Fonts.Arial));

                page.Header().Element(c => Header(c));
                page.Content().PaddingTop(16).Column(col =>
                {
                    col.Spacing(14);
                    col.Item().Element(c => ConfirmationBanner(c, booking));
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Element(c => GuestSection(c, booking.User));
                        row.ConstantItem(16);
                        row.RelativeItem().Element(c => AccommodationSection(c, booking.Accommodation));
                    });
                    col.Item().Element(c => ReservationSection(c, booking, nights, pricePerNight));
                    col.Item().Element(c => TermsSection(c));
                });
                page.Footer().Element(c => Footer(c));
            });
        });

        doc.GeneratePdf(filePath);
        return relativeUrl;
    }

    // ── Reusable section header ───────────────────────────────────────────

    private static void SectionHeading(ColumnDescriptor col, string title)
    {
        col.Item()
            .Background(ColorPrimaryLight)
            .PaddingHorizontal(10).PaddingVertical(7)
            .Text(title)
            .FontSize(8).Bold().FontColor(ColorPrimary);
    }

    private static void LabeledRow(ColumnDescriptor col, string label, string value, bool bold = false)
    {
        col.Item().Row(row =>
        {
            row.ConstantItem(110).Text(label).FontColor(ColorTextMuted);
            var cell = row.RelativeItem().Text(value);
            if (bold) cell.Bold();
        });
    }

    // ── Header ────────────────────────────────────────────────────────────

    private static void Header(IContainer c)
    {
        c.BorderBottom(2).BorderColor(ColorPrimary).PaddingBottom(10).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("Book4You").FontSize(26).Bold().FontColor(ColorPrimary);
                col.Item().Text("Accommodation Booking Service").FontSize(10).FontColor(ColorTextMuted);
            });
            row.AutoItem().AlignRight().AlignMiddle()
                .Text("BOOKING CONFIRMATION").FontSize(12).Bold().FontColor(ColorPrimary);
        });
    }

    // ── Confirmation banner ───────────────────────────────────────────────

    private static void ConfirmationBanner(IContainer c, Booking booking)
    {
        var statusBg = booking.Status == "Cancelled" ? "#ffcdd2" : "#c8e6c9";
        var statusFg = booking.Status == "Cancelled" ? "#b71c1c" : "#1b5e20";

        c.Background(ColorPrimary).Padding(14).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text($"Confirmation #BK{booking.Id:D6}")
                    .FontSize(15).Bold().FontColor(ColorWhite);
                col.Item().PaddingTop(3)
                    .Text($"Reserved on {booking.CreatedAt:dd MMMM yyyy} at {booking.CreatedAt:HH:mm}")
                    .FontSize(9).FontColor("#b2d8bc");
            });
            row.AutoItem().AlignRight().AlignMiddle()
                .Background(statusBg).Padding(6)
                .Text(booking.Status.ToUpperInvariant())
                .FontSize(10).Bold().FontColor(statusFg);
        });
    }

    // ── Guest section ─────────────────────────────────────────────────────

    private static void GuestSection(IContainer c, User user)
    {
        c.Column(col =>
        {
            SectionHeading(col, "GUEST INFORMATION");
            col.Item().Border(1).BorderColor(ColorBorder).Padding(12).Column(inner =>
            {
                inner.Spacing(6);
                LabeledRow(inner, "Full name:", $"{user.FirstName} {user.LastName}", bold: true);
                LabeledRow(inner, "Email:", user.Email);
                if (!string.IsNullOrWhiteSpace(user.Address))
                    LabeledRow(inner, "Address:", user.Address);
            });
        });
    }

    // ── Accommodation section ─────────────────────────────────────────────

    private static void AccommodationSection(IContainer c, Accommodation acc)
    {
        c.Column(col =>
        {
            SectionHeading(col, "ACCOMMODATION DETAILS");
            col.Item().Border(1).BorderColor(ColorBorder).Padding(12).Column(inner =>
            {
                inner.Spacing(6);
                LabeledRow(inner, "Property:", acc.Title, bold: true);
                LabeledRow(inner, "Type:", acc.Type);
                LabeledRow(inner, "Location:", acc.Location);
                if (!string.IsNullOrWhiteSpace(acc.Address))
                    LabeledRow(inner, "Address:", acc.Address);
                LabeledRow(inner, "Capacity:", $"Up to {acc.Capacity} guests");
            });
        });
    }

    // ── Reservation + pricing section ─────────────────────────────────────

    private static void ReservationSection(IContainer c, Booking booking, int nights, decimal pricePerNight)
    {
        var nightLabel = nights == 1 ? "night" : "nights";
        var guestLabel = booking.Guests == 1 ? "guest" : "guests";

        c.Column(col =>
        {
            SectionHeading(col, "RESERVATION DETAILS & PRICING");
            col.Item().Border(1).BorderColor(ColorBorder).Padding(12).Row(row =>
            {
                // Left: stay details
                row.RelativeItem().Column(inner =>
                {
                    inner.Spacing(7);
                    LabeledRow(inner, "Check-in:", booking.CheckInDate.ToString("ddd, dd MMM yyyy"), bold: true);
                    LabeledRow(inner, "Check-out:", booking.CheckOutDate.ToString("ddd, dd MMM yyyy"), bold: true);
                    LabeledRow(inner, "Duration:", $"{nights} {nightLabel}");
                    LabeledRow(inner, "Guests:", $"{booking.Guests} {guestLabel}");
                    LabeledRow(inner, "Status:", booking.Status);
                });

                // Vertical divider
                row.ConstantItem(1).Background(ColorBorder);
                row.ConstantItem(14);

                // Right: pricing breakdown
                row.RelativeItem().Column(inner =>
                {
                    inner.Spacing(6);
                    inner.Item().Text("PRICE BREAKDOWN").FontSize(8).Bold().FontColor(ColorPrimary);
                    inner.Item().Row(r =>
                    {
                        r.RelativeItem().Text($"€{pricePerNight:F2} × {nights} {nightLabel}").FontColor(ColorTextMuted);
                        r.AutoItem().Text($"€{booking.TotalPrice:F2}");
                    });
                    inner.Item().LineHorizontal(1).LineColor(ColorBorder);
                    inner.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Total").Bold().FontSize(13);
                        r.AutoItem().Text($"€{booking.TotalPrice:F2}").Bold().FontSize(13).FontColor(ColorPrimary);
                    });
                    inner.Item().PaddingTop(4).Row(r =>
                    {
                        r.RelativeItem().Text("Payment status:").FontColor(ColorTextMuted);
                        r.AutoItem().Text("Pending confirmation").FontColor("#c0392b");
                    });
                });
            });
        });
    }

    // ── Terms section ─────────────────────────────────────────────────────

    private static void TermsSection(IContainer c)
    {
        c.Column(col =>
        {
            SectionHeading(col, "BOOKING TERMS & CANCELLATION POLICY");
            col.Item().Border(1).BorderColor(ColorBorder).Padding(12).Column(inner =>
            {
                inner.Spacing(5);
                foreach (var line in new[]
                {
                    "• Free cancellation is available — contact us before your check-in date to cancel your reservation.",
                    "• Bookings are subject to availability and the property may cancel in exceptional circumstances.",
                    "• Please present this confirmation document upon arrival at the accommodation.",
                    "• For questions or support, use the Help Center or Contact page on our website.",
                    "• Book4You acts as an intermediary platform and is not liable for issues arising at the property."
                })
                {
                    inner.Item().Text(line).FontSize(9).FontColor("#444444");
                }
            });
        });
    }

    // ── Footer ────────────────────────────────────────────────────────────

    private static void Footer(IContainer c)
    {
        c.BorderTop(1).BorderColor(ColorBorder).PaddingTop(8).Row(row =>
        {
            row.RelativeItem()
                .Text("Thank you for choosing Book4You!")
                .FontSize(9).Italic().FontColor(ColorTextMuted);
            row.AutoItem().AlignRight().Text(x =>
            {
                x.Span("Page ").FontSize(9).FontColor(ColorTextMuted);
                x.CurrentPageNumber().FontSize(9).FontColor(ColorTextMuted);
                x.Span(" of ").FontSize(9).FontColor(ColorTextMuted);
                x.TotalPages().FontSize(9).FontColor(ColorTextMuted);
            });
        });
    }
}
