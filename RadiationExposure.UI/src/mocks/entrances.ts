import { Entrance } from '../models/types';

export const entrances: Entrance[] = [
  {
    entranceId: "1",
    workerId: "1",
    workerFirstName: "John",
    workerLastName: "Doe",
    entranceDate: new Date(),
    radiation: 0.1,
    zone: "30a",
    zoneId: "1",
  },
  {
    entranceId: "2",
    workerId: "2",
    workerFirstName: "Jane",
    workerLastName: "Doe",
    entranceDate: new Date(),
    radiation: 0.2,
    zone: "30b",
    zoneId: "2",
  },
  {
    entranceId: "3",
    workerId: "3",
    workerFirstName: "Alice",
    workerLastName: "Doe",
    entranceDate: new Date(),
    radiation: 1.5,
    zone: "204d",
    zoneId: "3",
  }
]