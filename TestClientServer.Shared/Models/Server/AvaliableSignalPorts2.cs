namespace TestClientServer.Shared.Models;

public class AvailableSignalPorts2
{
    public int Id { get; set; }

    public string Fdh { get; set; }

    public string PonPath { get; set; }

    public int? OntCount { get; set; }

    public int? AvailableOnts { get; set; }

    public int? NextAvailableOnt { get; set; }

    public string NextOntEquid { get; set; }

    public string Splitter { get; set; }

    public string SplitterCard { get; set; }

    public bool? SplitterInstalled { get; set; }

    public int? SplitterTailCount { get; set; }

    public int? AvailableTails { get; set; }

    public int? NextAvailableTail { get; set; }

    public string NextTailEquid { get; set; }

    public int? Olt { get; set; }

    public int? Lt { get; set; }

    public int? Pon { get; set; }

    public string Town { get; set; }
}