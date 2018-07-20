# Yablocko P2P application

This is simple and dirty implementation of blockhain currency (called yacoin here) based on the article: [Building A Blockchain In .NET Core - P2P Network](https://www.c-sharpcorner.com/article/building-a-blockchain-in-net-core-p2p-network/)

Every application instance combains server and client roles.

To start new instance you need to specify server port and user name.

E.g.:

```(sh)
dotnet run -- 7001 UserName
```

## Example scenario

Let's take a look at one simple scenario:

### 1. Start application for user A

--------- First screen ---------

```(sh)
> dotnet run -- 7001 A
Started server at ws://127.0.0.1:7001
Current user is A
=========================
1. Connect to a server
2. Add a transaction
3. Display Blockchain
4. Exit
=========================
Please select an action
```

### 2. Start application for user B

--------- Second screen ---------

```(sh)
> dotnet run -- 7002 B
Started server at ws://127.0.0.1:7002
Current user is B
=========================
1. Connect to a server
2. Add a transaction
3. Display Blockchain
4. Exit
=========================
Please select an action
```

### 3. Connect B to A

--------- Second screen ---------

```(sh)
Please select an action
1
Please enter the server URL:
ws://127.0.0.1:7001
Please select an action
Hi Client
```

--------- First screen ---------

```(sh)
Please select an action
Hi Server
```

### 4. Send 11 coins from B to A

--------- Second screen ---------

```(sh)
Please select an action
Hi Client
2
Please enter the receiver name:
A
Please enter the amount
11
Please select an action
```

### 5. Display blockchain

--------- First screen ---------

```(sh)
Please select an action
Hi Server
3
Blockchain
{
  "Chain": [
    {
      "Index": 1,
      "TimeStamp": "2018-07-20T14:23:02.4638348+01:00",
      "PreviousHash": "00HEFoj4vjP6JDnKtbabk8w7n3QduPuMkr69SusSMUc=",
      "Hash": "00YudLVd4c8PBMjC4PE/rPObCpANZdE2oI1etw63oIs=",
      "Transactions": [
        {
          "FromAddress": "B",
          "ToAddress": "A",
          "Amount": 11
        }
      ],
      "Nonce": 3558
    },
    {
      "Index": 0,
      "TimeStamp": "2018-07-20T14:17:39.93494+01:00",
      "PreviousHash": "",
      "Hash": "00HEFoj4vjP6JDnKtbabk8w7n3QduPuMkr69SusSMUc=",
      "Transactions": [],
      "Nonce": 204
    }
  ],
  "Difficulty": 2,
  "Reward": 1,
  "PendingTransactions": [
    {
      "FromAddress": "",
      "ToAddress": "B",
      "Amount": 1
    }
  ]
}
Please select an action
```

--------- Second screen ---------

```(sh)
Please select an action
3
Blockchain
{
  "Chain": [
    {
      "Index": 1,
      "TimeStamp": "2018-07-20T14:23:02.4638348+01:00",
      "PreviousHash": "00HEFoj4vjP6JDnKtbabk8w7n3QduPuMkr69SusSMUc=",
      "Hash": "00YudLVd4c8PBMjC4PE/rPObCpANZdE2oI1etw63oIs=",
      "Transactions": [
        {
          "FromAddress": "B",
          "ToAddress": "A",
          "Amount": 11
        }
      ],
      "Nonce": 3558
    },
    {
      "Index": 0,
      "TimeStamp": "2018-07-20T14:17:39.93494+01:00",
      "PreviousHash": "",
      "Hash": "00HEFoj4vjP6JDnKtbabk8w7n3QduPuMkr69SusSMUc=",
      "Transactions": [],
      "Nonce": 204
    }
  ],
  "Difficulty": 2,
  "Reward": 1,
  "PendingTransactions": [
    {
      "FromAddress": "",
      "ToAddress": "B",
      "Amount": 1
    }
  ]
}
```
