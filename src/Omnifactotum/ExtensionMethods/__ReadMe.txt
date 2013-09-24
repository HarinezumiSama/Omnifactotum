NOTE: If you're adding a new class containing extension methods:
    1. It MUST be manually placed under the namespace of the type being extended.
    2. It MUST have Omnifactotum prefix.

Example: If you're extending System.IO.Stream, the class has to be:
    a) put to System.IO namespace; and
    b) named OmnifactotumStreamExtensions.