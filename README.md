# XYZ Bornes – IoT Energy Station

## Description

XYZ Bornes is an IoT-based energy station simulator designed for
electricity and water consumption management in port environments.

The system simulates multiple energy stations, publishes telemetry
through MQTT, stores consumption data in PostgreSQL and applies
progressive pricing and threshold-based alerts.

## Architecture

- .NET 8
- C#
- PostgreSQL
- MQTT / Mosquitto
- MQTTnet
- Npgsql

## Projects

- `Xyz.Bornes.Domaine` – Domain entities and interfaces
- `Xyz.Bornes.Infrastructure` – PostgreSQL and MQTT implementations
- `Xyz.Bornes.Simulateur` – Energy station simulator

## DevOps Roadmap

- [x] Git repository
- [ ] Automated tests
- [ ] Docker
- [ ] Docker Compose
- [ ] CI/CD
- [ ] Security scanning
- [ ] Monitoring
- [ ] Deployment