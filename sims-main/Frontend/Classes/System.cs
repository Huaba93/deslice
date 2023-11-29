using System;
using SIMS_Frontend;
using Newtonsoft.Json;
using System.Net;
namespace SIMS_Frontend.Classes
{
	public class SystemService
	{

      
        public static SystemReturnObj GetSystems(string authToken)
        {
            SystemReturnObj retObj = new();
            string systemsJson = Backend.GetJsonFromBackend("/system", authToken);
            if (int.TryParse(systemsJson,out int statuscode)) { return new SystemReturnObj(statuscode); }
            List<System> systems = JsonConvert.DeserializeObject<List<System>>(systemsJson) ?? new();
            retObj.Systems = systems;
            string systemTypeJson = Backend.GetJsonFromBackend("/SystemType", authToken);
            if (int.TryParse(systemTypeJson, out int st_statuscode)) { return new SystemReturnObj(st_statuscode); }
            List<Systemtype> systemtypes = JsonConvert.DeserializeObject<List<Systemtype>>(systemTypeJson) ?? new() ;
            retObj.Systemtypes = systemtypes;
            string ManufactorJson = Backend.GetJsonFromBackend("/Manufactor", authToken);
            if (int.TryParse(ManufactorJson, out int manu_statuscode)) { return new SystemReturnObj(manu_statuscode); }
            List<Manufactor> manufactors = JsonConvert.DeserializeObject<List<Manufactor>>(ManufactorJson) ?? new();
            retObj.Manufactors = manufactors;
            return retObj;
        }

        
    }
    public class SystemReturnObj
    {
        public int Statuscode { get; set; } = 200;
        public string? Message { get; set; }
        public List<System>? Systems { get; set; }
        public List<Systemtype>? Systemtypes { get; set; }
        public List<Manufactor>? Manufactors { get; set; }
        public SystemReturnObj() { }
        public SystemReturnObj(int statuscode) { this.Statuscode = statuscode; }

        public SystemReturnObj(int statuscode, List<System> systems) { this.Statuscode = statuscode; this.Systems = Systems; }
    }
    public class System
    {
        public int SystemId { get; set; }
        public string? Hostname { get; set; }
        public int ManufactorID { get; set; }
        public int SystemtypeID { get; set; }
        public string? SerialNumber { get; set; }
        public string? IpAddress { get; set; }
        public string? Location { get; set; }
        public int? Criticality { get; set; }

    }

    public class Systemtype
    {
        public int SystemTypeId { get; set; }
        public string Name { get; set; }
    }
    public class Manufactor
    {
        public int ManufactorId { get; set; }
        public string Name { get; set; }
    }
}

