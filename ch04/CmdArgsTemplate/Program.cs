using CmdArgsTemplate;
using CommandLine;
using CommandLine.Text;

var results = Parser.Default.ParseArguments<Options>(args)
  .WithParsed<Options>(options => {
    WriteLine(options.Text);
  });

results.WithNotParsed(_ =>
  WriteLine(HelpText.RenderUsageText(results)));
