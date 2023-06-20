using System.Diagnostics;
using System.Reflection;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Porcupine.Shaders;

public readonly ref struct ShaderCompiler {

	private static readonly Assembly _assembly = typeof(ShaderCompiler).Assembly;

	public ShaderCompiler() {
		_program = GL.CreateProgram();
		_shaders = new(4);
	}

	private readonly ProgramHandle _program;
	private readonly List<ShaderHandle> _shaders;

	public void Compile(string filename, ShaderType type) {
		if (_assembly.GetManifestResourceStream($"Porcupine.Shaders.{filename}") is not Stream stream) {
			Debug.WriteLine($"Could not locate {filename}");
			throw new FileNotFoundException(filename);
		}

		string code = new StreamReader(stream).ReadToEnd();

		ShaderHandle shader = GL.CreateShader(type);
		GL.ShaderSource(shader, code);
		GL.CompileShader(shader);

		GL.GetShaderInfoLog(shader, out string status);
		if (!string.IsNullOrEmpty(status))
			Debug.WriteLine($"[Shader] {filename} is {status}");

		_shaders.Add(shader);
		GL.AttachShader(_program, shader);
	}

	public void CompileVertex(string filename)
		=> Compile(filename + ".vert", ShaderType.VertexShader);

	public void CompileGeometry(string filename)
		=> Compile(filename + ".geom", ShaderType.GeometryShader);

	public void CompileFragment(string filename)
		=> Compile(filename + ".frag", ShaderType.FragmentShader);

	public void Finalize(out ProgramHandle program) {
		GL.LinkProgram(_program);

		foreach (ShaderHandle shader in _shaders) {
			GL.DetachShader(_program, shader);
			GL.DeleteShader(shader);
		}

		program = _program;
	}

}
