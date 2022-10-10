# infra-http
Infrastructure components to work with Kafka

appsettings.json example:

    {
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
            }   
        },
        "KafkaOptions" : {
            "BootstrapServers" : "localhost:9092",
            "GroupId" : "cache-update-hosted-service",
            "Topic" : "order_events",
            "AutoOffsetReset" : "Earliest",
            "EnableAutoCommit" : false,
            "EnableAutoOffsetStore" : true
        },
        "RetryPolicyOptions" : {
            "RetryCount" : 2,
            "MedianFirstRetryDelay" : "00:00:01"
        },
        "AllowedHosts": "*"
    }