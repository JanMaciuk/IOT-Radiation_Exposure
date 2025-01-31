export type Employee = {
  id: string;
  name: string;
  surname: string;
  lastEntranceDate: Date | null;
  lastZoneId: number | null;
  lastZoneName: string | null;
  radiationDoseInLastMonth: number;
  radiationDoseInLastWeek: number;
}

export type Zone = {
  id: string;
  zoneName: string;
  radiation: number;
  lastEntrance: Date | null;
  employeesInsideNow: number;
}

export type Entrance = {
  id: string;
  zoneId: number;
  zoneName: string;
  employeeName: number;
  worker: Employee;
  entryTime: Date;
  exitTime: Date | null;
  duration: number; 
  radiationDose: number;
}

export type DashboardStats = {
  totalEmployees: number
  totalZones: number
  entrancesToday: number
  workersAboveRadiationLimit: number
};