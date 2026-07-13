using System.Globalization;
using NPOI.SS.UserModel;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public static class ProductImportWorkbook
{
    private static readonly string[] RequiredHeaders =
    [
        "Titulo",
        "LoginVendedor",
        "PrecoAVista",
        "Sku",
        "Estoque",
        "EhOferta",
        "Categoria",
        "Subcategoria",
        "Descritivo",
        "Imagens"
    ];

    public static List<ProductImportRow> ReadRows(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        var workbook = WorkbookFactory.Create(stream);
        var sheet = workbook.GetSheet("Produtos") ?? workbook.GetSheetAt(0);
        var headerRow = sheet.GetRow(sheet.FirstRowNum) ?? throw new ProductImportException("Planilha sem cabecalho.");
        var headers = ReadHeaders(headerRow);
        ValidateHeaders(headers);

        var rows = new List<ProductImportRow>();
        var formatter = new DataFormatter(CultureInfo.GetCultureInfo("pt-BR"));
        for (var rowIndex = sheet.FirstRowNum + 1; rowIndex <= sheet.LastRowNum; rowIndex++)
        {
            var row = sheet.GetRow(rowIndex);
            if (row is null || IsEmpty(row, formatter, headers.Count))
            {
                continue;
            }

            rows.Add(ReadRow(row, formatter, headers));
        }

        return rows;
    }

    public static string NormalizeKey(string value) =>
        string.Join(' ', value.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    private static ProductImportRow ReadRow(IRow row, DataFormatter formatter, IReadOnlyList<ProductImportHeader> headers)
    {
        string Cell(string name) => formatter.FormatCellValue(row.GetCell(headers.First(header => header.Name == name).Index)).Trim();
        var parsed = new ProductImportRow
        {
            RowNumber = row.RowNum + 1,
            Title = Cell("Titulo"),
            LoginVendedor = Cell("LoginVendedor"),
            Sku = Cell("Sku"),
            Category = Cell("Categoria"),
            SubCategory = Cell("Subcategoria"),
            Description = Cell("Descritivo"),
            ImageUrls = Cell("Imagens").Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList()
        };

        parsed.Price = ParseDecimal(Cell("PrecoAVista"), parsed.RowNumber);
        parsed.Stock = ParseInt(Cell("Estoque"), parsed.RowNumber, "Estoque");
        parsed.Offer = ParseBool(Cell("EhOferta"), parsed.RowNumber);

        foreach (var header in headers.Where(header => header.AttributeName is not null))
        {
            var value = formatter.FormatCellValue(row.GetCell(header.Index)).Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                parsed.Attributes[header.AttributeName!] = value;
            }
        }

        return parsed;
    }

    private static List<ProductImportHeader> ReadHeaders(IRow headerRow)
    {
        var headers = new List<ProductImportHeader>();
        for (var index = 0; index < headerRow.LastCellNum; index++)
        {
            var value = headerRow.GetCell(index)?.StringCellValue?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var canonical = CanonicalHeader(value);
            var attributeName = value.StartsWith("Atributo:", StringComparison.OrdinalIgnoreCase)
                ? value["Atributo:".Length..].Trim()
                : null;
            headers.Add(new ProductImportHeader(index, canonical, attributeName));
        }

        return headers;
    }

    private static void ValidateHeaders(IReadOnlyList<ProductImportHeader> headers)
    {
        var names = headers.Select(header => header.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = RequiredHeaders.Where(header => !names.Contains(header)).ToArray();
        if (missing.Length > 0)
        {
            throw new ProductImportException($"Cabecalhos obrigatorios ausentes: {string.Join(", ", missing)}.");
        }

        var duplicateAttributes = headers
            .Where(header => header.AttributeName is not null)
            .GroupBy(header => NormalizeKey(header.AttributeName!))
            .Where(group => group.Count() > 1)
            .Select(group => group.First().AttributeName)
            .ToArray();
        if (duplicateAttributes.Length > 0)
        {
            throw new ProductImportException($"Atributos duplicados: {string.Join(", ", duplicateAttributes)}.");
        }
    }

    private static string CanonicalHeader(string value)
    {
        var normalized = NormalizeKey(value).Replace(" ", string.Empty, StringComparison.Ordinal);
        return normalized switch
        {
            "titulo" => "Titulo",
            "loginvendedor" => "LoginVendedor",
            "precoavista" => "PrecoAVista",
            "sku" => "Sku",
            "estoque" => "Estoque",
            "ehoferta" or "eoferta" => "EhOferta",
            "categoria" => "Categoria",
            "subcategoria" => "Subcategoria",
            "descritivo" or "descricao" => "Descritivo",
            "imagens" => "Imagens",
            _ when value.StartsWith("Atributo:", StringComparison.OrdinalIgnoreCase) => value,
            _ => value
        };
    }

    private static bool IsEmpty(IRow row, DataFormatter formatter, int cellCount)
    {
        for (var index = 0; index < cellCount; index++)
        {
            if (!string.IsNullOrWhiteSpace(formatter.FormatCellValue(row.GetCell(index))))
            {
                return false;
            }
        }

        return true;
    }

    private static decimal ParseDecimal(string value, int rowNumber)
    {
        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out var parsedValue) ||
            decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out parsedValue))
        {
            return parsedValue;
        }

        throw new ProductImportException("PrecoAVista invalido.", rowNumber);
    }

    private static int ParseInt(string value, int rowNumber, string field)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new ProductImportException($"{field} invalido.", rowNumber);
    }

    private static bool ParseBool(string value, int rowNumber)
    {
        return NormalizeKey(value) switch
        {
            "sim" or "true" or "1" or "s" => true,
            "nao" or "nÃƒÂ£o" or "false" or "0" or "n" => false,
            _ => throw new ProductImportException("EhOferta invalido.", rowNumber)
        };
    }
}

