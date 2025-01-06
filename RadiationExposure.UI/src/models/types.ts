export type Employee = {
  id: string;
  name: string;
  surname: string;
  lastEntranceDate: Date | null;
  lastZoneId: number | null;
  lastZoneName: string | null; 
}

export type Zone = {
  id: string;
  name: string;
  radiation: number;
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