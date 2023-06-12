using System.ComponentModel.DataAnnotations;

namespace MoneyTransfer;

public class Account
{
  [Key]
  public string AccountNumber { get; set; }
  public decimal Balance { get; set; }

  public Account(string accountNumber, decimal balance)
  {
    AccountNumber = accountNumber;
    Balance = balance;
  }
}
