![Docker Image Version](https://img.shields.io/docker/v/blueclikk/kwik-nesta.workers.svc?sort=semver&label=version)
![Docker Pulls](https://img.shields.io/docker/pulls/blueclikk/kwik-nesta.workers.svc)

# Kwik Nesta Worker Service

The **Worker Service** is a background processing service for Kwik Nesta.
It handles scheduled and asynchronous tasks such as message queue consumption, event processing, and background job execution to keep the system responsive and reliable.

## 🚀 Features

* Runs as a long-lived background process.
* Consumes messages from RabbitMQ and other queues.
* Executes periodic and scheduled background jobs.
* Integrated with logging, monitoring, and observability.
* Designed for scalability and fault tolerance.

## 🛠️ Tech Stack

* **.NET 8 Worker Service** template
* **RabbitMQ** for message queue consumption
* **Serilog** for structured logging
* **OpenTelemetry** for tracing and metrics

## ⚡ Getting Started

### Prerequisites

* .NET 8 SDK
* RabbitMQ running locally or via Docker

## 📜 License

MIT License © Kwik Nesta