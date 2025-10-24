namespace EmployeeManagement.Cadastro.Application.Helpers;

public static class EmailHelper
{
    /// <summary>
    /// Gera um email corporativo baseado no nome do funcionário.
    /// Converte o nome para minúsculas e substitui espaços por pontos.
    /// Exemplo: "João Silva" -> "joao.silva@empresa.com"
    /// </summary>
    /// <param name="employeeName">Nome completo do funcionário</param>
    /// <param name="domain">Domínio do email (padrão: "empresa.com")</param>
    /// <returns>Email corporativo formatado</returns>
    public static string GenerateCompanyEmail(string employeeName, string domain = "empresa.com")
    {
        if (string.IsNullOrWhiteSpace(employeeName))
        {
            throw new ArgumentException("Nome do funcionário não pode ser vazio", nameof(employeeName));
        }

        var emailPrefix = employeeName.ToLower().Replace(" ", ".");
        return $"{emailPrefix}@{domain}";
    }
}
