Prefer assignment-level simplicity over production-style consensus helpers unless the brief requires them; PoS UX fix: forge tx list using selected validator address; adaptive difficulty skips PoS intervals.
PoW: require miner address before building the transaction list; PoS: validate selectionProof matches previous hash + timestamp + validator; increment blocksForged on accepted PoS; Validator.penalties camelCase.
PR babysit: repo has no GH checks—run Release MSBuild locally; treat deterministic multi-node consensus comments as out-of-scope for offline assignment unless rubric requires them.

