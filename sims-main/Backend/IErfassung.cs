using System;
namespace SIMS_Backend
{
	public interface IErfassung
	{
		List<System> GetSystems();
		System SearchSystem(string? ipAddress, string? hostname, string? serialNumber, string? macAddress);
		List<Systemtype> GetSystemtypes();
	}

}

