using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Marketplace.Api.Features.Admin.Produto.ProductImports.Shared;

public static class ProductImportTemplate
{
    public static byte[] Create()
    {
        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet("Produtos");
        var headers = new[]
        {
            "Titulo",
            "LoginVendedor",
            "PrecoAVista",
            "Sku",
            "Estoque",
            "EhOferta",
            "Categoria",
            "Subcategoria",
            "Descritivo",
            "Imagens",
            "Atributo: Marca",
            "Atributo: Modelo",
            "Atributo: Autor",
            "Atributo: Editora",
            "Atributo: Processador",
            "Atributo: Memoria RAM",
            "Atributo: Volume",
            "Atributo: Cordas"
        };
        var headerRow = sheet.CreateRow(0);
        for (var index = 0; index < headers.Length; index++)
        {
            headerRow.CreateCell(index).SetCellValue(headers[index]);
            sheet.SetColumnWidth(index, 22 * 256);
        }

        var examples = new[]
        {
            new[] { "Codigo Limpo", "techstore", "79,90", "IMP-LIVRO-CLEAN-CODE", "15", "sim", "Livros", "Tecnologia", "Boas praticas para escrever codigo legivel, testavel e sustentavel.", "https://placehold.co/900x900.jpg?text=Codigo+Limpo", "Alta Books", "", "Robert C. Martin", "Alta Books", "", "", "", "" },
            new[] { "Notebook Acer Predator Helios 300", "techstore", "7499,00", "IMP-NOTE-ACER-PREDATOR", "4", "sim", "Informatica", "Notebooks", "Notebook gamer com alto desempenho para jogos, desenvolvimento e criacao.", "https://placehold.co/900x900.jpg?text=Acer+Predator", "Acer", "Predator Helios 300", "", "", "Intel Core i7", "16 GB", "", "" },
            new[] { "Violao Tagima Dallas Tuner Eletroacustico", "multisom", "899,90", "IMP-VIOL-TAGIMA-DALLAS", "11", "sim", "Instrumentos musicais", "Violoes", "Violao com afinador embutido, cordas de aco e otima resposta para estudo e palco.", "https://placehold.co/900x900.jpg?text=Tagima+Dallas", "Tagima", "Dallas Tuner", "", "", "", "", "", "Aco" },
            new[] { "Coca-Cola Zero Acucar 350ml", "techstore", "4,99", "IMP-BEB-COCA-ZERO-350", "120", "nao", "Bebidas", "Refrigerantes", "Refrigerante zero acucar em lata de 350 ml.", "https://placehold.co/900x900.jpg?text=Coca-Cola+Zero", "Coca-Cola", "", "", "", "", "", "350 ml", "" }
        };

        for (var rowIndex = 0; rowIndex < examples.Length; rowIndex++)
        {
            var row = sheet.CreateRow(rowIndex + 1);
            for (var cellIndex = 0; cellIndex < examples[rowIndex].Length; cellIndex++)
            {
                row.CreateCell(cellIndex).SetCellValue(examples[rowIndex][cellIndex]);
            }
        }

        using var stream = new MemoryStream();
        workbook.Write(stream, true);
        return stream.ToArray();
    }
}

