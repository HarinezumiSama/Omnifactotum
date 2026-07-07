# [Omnifactotum Compiler Extensions](./README.md)

## Analyzer Rule `OFCA0014`

- `Validation constraint type is not compatible with the type of the validated value`
  - A validation constraint that derives from `Omnifactotum.Validation.Constraints.TypedMemberConstraintBase<T>` can only validate values of type `T`. The analyzer verifies that the constraint is applied to a compatible member:
    - For `MemberConstraintAttribute` / `MemberConstraintAttribute<TMemberConstraint>`, the type of the annotated member must be compatible with (that is, implicitly convertible via an identity, reference, or boxing conversion to) `T`.
    - For `MemberItemConstraintAttribute` / `MemberItemConstraintAttribute<TMemberConstraint>`, the item type of the annotated collection member must be compatible with `T`.
  - Otherwise, the constraint would fail at run time with an `InvalidOperationException` when it attempts to cast the value to `T`.
  - For example, the following annotations are invalid:
    - A `bool` property is not compatible with the `string` expected by the constraint:
      ```csharp
      [MemberConstraint<NotNullConstraint<string>>]
      public bool Value { get; set; }
      ```
    - A `bool` item is not compatible with the `string` expected by the constraint:
      ```csharp
      [MemberItemConstraint(typeof(NotNullConstraint<string>))]
      public bool[] Value { get; set; }
      ```
    - A `Func<T, bool>` delegate is not compatible with the `Action` delegate expected by the constraint, and no substitution of the type parameter `T` can make it compatible:
      ```csharp
      public class SomeClass<T>
      {
          [MemberConstraint<NotNullConstraint<Action>>]
          public Func<T, bool>? IsSomethingSet { get; set; }
      }
      ```

### Additional Information

- When required, a diagnostic rule can be:
  - suppressed using [#pragma warning](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#pragma-warning); or
  - suppressed using [SuppressMessageAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.suppressmessageattribute); or
  - ignored using the [NoWarn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings#nowarn) project property.
