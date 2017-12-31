


|------------------------------------------|
| CO2.RetroCPL COMPILER OPTIMIZER 2 RETROC |
| v1.00, December 2017                     |
|                                          |
| Author: Emilio Arango Delgado de Mendoza |
|------------------------------------------|

The .csproj of this C# project has been modified to add a custom build process.
Next lines has been added to the Targets in order of generating the scaner and the parser using the GardensPoint tools:

------------------------------------------

  <PropertyGroup>
    <CoreCompileDependsOn>GenerateLexer;$(CoreCompileDependsOn)</CoreCompileDependsOn>
    <CoreCompileDependsOn>GenerateParser;$(CoreCompileDependsOn)</CoreCompileDependsOn>
    <UseHostCompilerIfAvailable>false</UseHostCompilerIfAvailable>
  </PropertyGroup>
  <Target Name="GenerateLexer" Inputs="@(Lex)" Outputs="@(Lex->'%(Filename).cs')">
    <Exec Command="$(ProjectDir)GardensPoint/gplex.exe /unicode /verbose /parser /stack /minimize /classes /compressMap /compressNext /persistBuffer /embedbuffers /out:@(Lex ->'%(Filename).cs') %(Lex.Identity)" />
    <CreateItem Include="%(Lex.Filename).cs">
      <Output TaskParameter="Include" ItemName="FileWrites" />
    </CreateItem>
  </Target>
  <Target Name="GenerateParser" Inputs="@(Yacc)" Outputs="@(Yacc->'%(Filename).cs')">
    <Exec Command="$(ProjectDir)GardensPoint/gppg.exe /conflicts /no-lines /gplex /verbose %(Yacc.Identity) &gt; @(Yacc ->'%(Filename).cs')" />
    <CreateItem Include="%(Yacc.Filename).cs">
      <Output TaskParameter="Include" ItemName="FileWrites" />
    </CreateItem>
  </Target>

------------------------------------------
