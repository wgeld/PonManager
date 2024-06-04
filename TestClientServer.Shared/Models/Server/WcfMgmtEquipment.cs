using System.Collections;

namespace TestClientServer.Shared.Models;
public class WcfMgmtEquipment
{
    public string LocationId { get; set; }

    public string EquClass { get; set; }

    public string EquId { get; set; }

    public string Serial { get; set; }

    public string Mac { get; set; }

    public string AssetTag { get; set; }

    public string Manufacturer { get; set; }

    public string Model { get; set; }

    public string Ssid24 { get; set; }

    public string Ssid5g { get; set; }

    public string SsidPassword { get; set; }

    public string Fdh { get; set; }

    public string SplitterCard { get; set; }

    public int? SplitterTail { get; set; }

    public int? Olt { get; set; }

    public int? Lt { get; set; }

    public int? Pon { get; set; }

    public int? Ont { get; set; }

    public DateTime? InstallDate { get; set; }

    public DateTime? RemovalDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }

    public string RemovedBy { get; set; }

    public int Id { get; set; }

    public string Town { get; set; }

    public string ContainerId { get; set; }
    
}