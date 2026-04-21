# Implementation Plan

- [completed] Reproduce the current failure modes and baseline behavior with a small external regression harness.
- [completed] Fix adaptive difficulty parsing and hot-path proof-of-work checks with minimal helper-driven changes.
- [completed] Remove repeated difficulty-prefix list allocations during chain validation.
- [completed] Fix adjacent correctness issues in balance calculation, transaction validation, and blank miner input handling.
- [completed] Run a targeted `deslop` pass on the touched diff only.
- [completed] Verify with a Release build and targeted runtime checks, then document results below.

# Review

- `AdaptiveDifficulty`: malformed or wrong-length hash strings now fail cleanly instead of throwing; mining precomputes the target once and compares raw SHA-256 bytes instead of reparsing hex per nonce.
- `Block` and `Blockchain`: proof-of-work validation still uses the same difficulty model, but validation no longer allocates prefix lists on every iteration, and miner balances no longer double-count the reward transaction already stored in the block.
- `Transaction` and `Wallet`: transaction validation is now side-effect free, and malformed signature material returns `false` instead of surfacing validation exceptions.
- `BlockchainApp`: mining now rejects a blank miner public ID before starting, and the benchmark handler was rewritten to stay compatible with the full-framework compiler used by the project build.
- Verification: `C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ..\BlockchainAssignment.sln /p:Configuration=Release` succeeded; a throwaway regression harness passed wallet, transaction, mining, validation, malformed-hash, and balance checks; `bin\Release\BlockchainAssignment.exe` launched successfully and stayed running until closed.
