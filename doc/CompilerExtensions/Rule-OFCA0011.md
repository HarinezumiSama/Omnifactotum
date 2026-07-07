# [Omnifactotum Compiler Extensions](./README.md)

## Analyzer Rule `OFCA0011`

- `Validation constraint type does not implement the required interface 'IMemberConstraint'`
  - A validation constraint type specified via `typeof(...)` in a `MemberConstraintAttribute` or `MemberItemConstraintAttribute` must implement the `Omnifactotum.Validation.Constraints.IMemberConstraint` interface.
