namespace MoneyTransfer;

public class InsufficientFundsException : Exception
{
  public InsufficientFundsException(string message) :
    base(message) { }
}