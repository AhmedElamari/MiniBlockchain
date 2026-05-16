# MiniBlockchain Assignment

## Overview
This repository contains a simple implementation of a blockchain application in C# (.NET Framework 4.8), designed as an educational assignment. The project simulates a basic blockchain system with fundamental concepts such as blocks, cryptographic hashing, wallets, transactions, and validation mechanisms.

## Core Components
- **Blockchain**: Manages the chain of blocks, difficulty, and consensus.
- **Block**: Represents individual blocks containing a list of transactions, previous hash, timestamp, and nonce.
- **Wallet**: Handles key generation (Public/Private keys) and transaction signatures to securely transfer funds.
- **Transaction**: Encapsulates sender, receiver, amount, and digital signatures.
- **Validator**: Validates transactions and blocks to maintain chain integrity.
- **HashTools**: Utility class for cryptographic hashing (e.g., SHA256).

## Environment
- Language: C# (.NET Framework 4.8)
- IDE: Visual Studio 2022
- UI: Windows Forms

## Usage
The application features a built-in GUI (`BlockchainApp.cs`) that allows you to:
- Generate Wallets
- Create and sign transactions
- Mine new blocks
- Validate the integrity of the blockchain
