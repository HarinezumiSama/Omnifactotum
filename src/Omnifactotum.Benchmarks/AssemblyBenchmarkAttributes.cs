using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

// Columns

[assembly: BaselineColumn]
[assembly: CategoriesColumn]
[assembly: KurtosisColumn]
//// [assembly: LogicalGroupColumn]
[assembly: MeanColumn]
[assembly: MedianColumn]
// [assembly: NamespaceColumn]
[assembly: StdDevColumn]

// Exporters

[assembly: PlainExporter]
[assembly: JsonExporter(indentJson: true)]
[assembly: HtmlExporter]
[assembly: MarkdownExporterAttribute.GitHub]
[assembly: RPlotExporter]

// Miscellaneous

[assembly: Orderer(SummaryOrderPolicy.FastestToSlowest/*, MethodOrderPolicy.Alphabetical*/)]
