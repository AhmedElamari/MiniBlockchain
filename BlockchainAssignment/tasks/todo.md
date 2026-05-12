# Implementation Plan

## PoS economics (validator stake, slashing, determinism)

- [completed] Validator: `IncrementPenalties`, `SlashStake`.
- [completed] Blockchain: slash on rejected PoS block for registered offender; deterministic `SelectValidator` from last-block hash × scaled stake weights; stake ≤ confirmed balance at registration; locked stake deducted in `getSpendableBalance`.
- [completed] UI: validators list shows penalties; validation evidence submits tampered PoS block and re-lists validators.
- [completed] Release MSBuild succeeds.
- [completed] Manual WinForms checklist (recommended before submission): mine/receive funds → reject stake above balance → register → spend blocked below locked stake → forge PoS → validation evidence slashes → repeat removals at 3 strikes or stake zero → validate chain.

## Earlier work (baseline)

- [completed] Reproduce the current failure modes and baseline behavior with a small external regression harness.
- [completed] Fix adaptive difficulty parsing and hot-path proof-of-work checks with minimal helper-driven changes.
- [completed] Remove repeated difficulty-prefix list allocations during chain validation.
- [completed] Fix adjacent correctness issues in balance calculation, transaction validation, and blank miner input handling.
- [completed] Run a targeted `deslop` pass on the touched diff only.
- [completed] Verify with a Release build and targeted runtime checks, then document results below.

# Review

## PoS economics (latest)

- `Validator`: `penalties` is incremented and stake reduced via slashing helpers; aligns with coursework BFT framing.
- `Blockchain`: rejected PoS blocks whose `validatorAddress` matches an existing validator apply 10% stake slash per offense, bump penalties, remove at 3 penalties or zero stake; `addValidator` requires stake ≤ confirmed on-chain balance; spendable balance subtracts locked stake for registered validators; validator selection uses `SHA-256(previous.Hash)` decoded as unsigned `BigInteger` modulo total scaled weights (stakes × 1e6), reproducible given the chain head.
- `BlockchainApp`: list validators shows penalties and stake; validation evidence exercises tampered PoS Merkle rejection and surfaces slashing outcome plus current validator snapshots.
- Verification: Release MSBuild succeeded for this milestone; interactive WinForms walkthrough follows checklist above (`bin\\Release\\BlockchainAssignment.exe`).

## Earlier milestones

- `AdaptiveDifficulty`: malformed or wrong-length hash strings now fail cleanly instead of throwing; mining precomputes the target once and compares raw SHA-256 bytes instead of reparsing hex per nonce.
- `Block` and `Blockchain`: proof-of-work validation still uses the same difficulty model, but validation no longer allocates prefix lists on every iteration, and miner balances no longer double-count the reward transaction already stored in the block.
- `Transaction` and `Wallet`: transaction validation is now side-effect free, and malformed signature material returns `false` instead of surfacing validation exceptions.
- `BlockchainApp`: mining now rejects a blank miner public ID before starting, and the benchmark handler was rewritten to stay compatible with the full-framework compiler used by the project build.
- Verification: `C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\MSBuild.exe ..\\BlockchainAssignment.sln /p:Configuration=Release` succeeded for earlier harness work; regression harness covered wallet/transaction/mining flows at that time.
