namespace TestClientServer.Shared.Models;

public class FormData
{
    private string _fsa;

    public string Fsa
    {
        get => _fsa;
        set
        {
            if (value.Length == 1)
            {
                _fsa = "0" + value;
            }
            else
            {
                _fsa = value;
            }
        }
    }
    public string Town { get; set; }
    public int Olt { get; set; }
    public int Pon { get; set; }
    public int Lt { get; set; }
    public string Splitter { get; set; }
    
}