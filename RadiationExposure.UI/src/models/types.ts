export type Entrance = {
  entranceId: string;
  workerId: string;
  workerFirstName: string;
  workerLastName: string;
  entranceDate: Date;
  exitDate?: Date;
  radiation: number;
  zone: string;
  zoneId: string;
};