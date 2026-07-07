# [Omnifactotum Compiler Extensions](./README.md)

## Analyzer Rule `OFCA0012`

- `Validation constraint type does not have a required parameterless constructor`
  - A validation constraint type specified via `typeof(...)` in a `MemberConstraintAttribute` or `MemberItemConstraintAttribute` must have a parameterless constructor so that it can be instantiated during validation.
