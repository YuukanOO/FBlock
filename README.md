# FBlock : Flow based programming for .Net

FBlock is a simple and tiny library to develop and run flow based applications.

You develop an application by using Jobs and Components. A component has only one input parameter and returns an output parameter, it must be generic to be reusable in other projects.

Basically, you make components and chain them using a Job. This is a simple example.

```charp

// The first component take an int as a parameter and returns a string
class MyFirstComponent : Component<int, string>
{
  // This where all functional stuff happenned
  // The context object is a dictionary where you can set and get everything
  // you want inside the same job.
  public override string Process(int arg, JobContext context)
  {
    return "value was: " + arg.ToString();
  }
}

// The second one take a string and returns it to uppercase
class MySecondComponent : Component<string, string>
{
  public override string Process(string arg, JobContext context)
  {
    return arg.ToUpper();
  }
}

// Now, let's make a job to use these components
Job<string, string> convertToStringUpper = new Job<string, string>();

convertToStringUpper
  .Start(new MyFirstComponent())
  // If you had other components, you would use the .Then method to chain them with type checking
  .End(new MySecondComponent());

// And then, launch the job
string result = convertToStringUpper.Process(5):

// Will yield "VALUE WAS: 5"
```

This sample is useless but show how this is done. Thanks to generics, type checking is done at compilation and you can't chain components with output < input not possible.

Here it is, time to develop **reusable components** and share them!