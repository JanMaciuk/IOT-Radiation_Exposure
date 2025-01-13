namespace IOT.Dto;

record EmployeeLastEntrance(
    DateTime? LastEntranceDate,
    int? LastZoneId,
    string? LastZoneName
);