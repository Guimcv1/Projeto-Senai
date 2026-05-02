using SCA.Back.Data;

namespace SCA.Tests;

public class DataConstantsTests
{
    [Fact]
    public void EstadosConstants_ShouldHaveExpectedValues()
    {
        // Assert
        Assert.Equal("Livre", Estados.Livre);
        Assert.Equal("Analise", Estados.Analise);
        Assert.Equal("Emprestado", Estados.Emprestado);
    }

    [Fact]
    public void AcaoTipoConstants_ShouldHaveExpectedValues()
    {
        // Assert
        Assert.Equal("Usuario", AcaoTipo.Usuario);
        Assert.Equal("Item", AcaoTipo.Item);
        Assert.Equal("Sala", AcaoTipo.Sala);
    }
}

