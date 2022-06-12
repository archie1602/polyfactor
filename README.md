
<h1 align="center">polyfactor</h1>

<h3 align="center">

[![NuGet](https://img.shields.io/nuget/v/polyfactor?color=https://www.nuget.org/packages/polyfactor/)](https://www.nuget.org/packages/polyfactor/)
[![GitHub](https://img.shields.io/github/license/archie1602/polyfactor?color=blue)](LICENSE.md)

</h3>

polyfactor is an open source, lightweight library whose main purpose is to solve polynomial factorization problem over $GF(p), GF(p^n), \mathbb{Q}$ fields. Currently, only factorization over $GF(p)$ is supported.

Implemented factorization methods:
1. Berlekamp
2. Cantor-Zassenhaus
3. Kaltofen-Shoup

## Usage
polyfactor library can be used in any .NET project that targets `.net standard 2.0`.
But the easiest way to use `polyfactor` library - `Jupyter Notebook` and `JupyterLab` notebooks. This library contains `polyfactor.Interactive` extension for `.NET Interactive` that allows to render polynomials in interactive notebooks. The math are rendered using `MathJax` library.

## License
The `polyfactor` is open source library and distributed under MIT license.