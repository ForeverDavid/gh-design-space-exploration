﻿using JMetalCSharp.Core;

namespace JMetalCSharp.Problems.WFG
{
	/// <summary>
	/// This class implements the WFG4 problem
	/// Reference: Simon Huband, Luigi Barone, Lyndon While, Phil Hingston
	///            A Scalable Multi-objective Test Problem Toolkit.
	///            Evolutionary Multi-Criterion Optimization: 
	///            Third International Conference, EMO 2005. 
	///            Proceedings, volume 3410 of Lecture Notes in Computer Science
	/// </summary>
	public class WFG4 : WFG
	{
		#region Constructors
		/// <summary>
		/// Creates a default WFG4 with 
		/// 2 position-related parameter,
		/// 4 distance-related parameter and
		/// 2 objectives
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		public WFG4(string solutionType)
			: this(solutionType, 2, 4, 2)
		{
		}

		/// <summary>
		/// Creates a WFG4 problem instance
		/// </summary>
		/// <param name="solutionType">The solution type must "Real" or "BinaryReal".</param>
		/// <param name="k">Number of position parameters</param>
		/// <param name="l">Number of distance parameters</param>
		/// <param name="M">Number of objective functions</param>
		public WFG4(string solutionType, int k, int l, int M)
			: base(solutionType, k, l, M)
		{
			ProblemName = "WFG4";

			S = new int[M];
			for (int i = 0; i < M; i++)
			{
				S[i] = 2 * (i + 1);
			}

			A = new int[M - 1];
			for (int i = 0; i < M - 1; i++)
			{
				A[i] = 1;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// WFG4 t1 transformation
		/// </summary>
		/// <param name="z"></param>
		/// <param name="k"></param>
		/// <returns></returns>
		public float[] T1(float[] z, int k)
		{
			float[] result = new float[z.Length];

			for (int i = 0; i < z.Length; i++)
			{
				result[i] = Transformations.S_multi(z[i], 30, 10, (float)0.35);
			}

			return result;
		}

		/// <summary>
		/// WFG4 t2 transformation
		/// </summary>
		/// <param name="z"></param>
		/// <param name="k"></param>
		/// <param name="M"></param>
		/// <returns></returns>
		public float[] T2(float[] z, int k, int M)
		{
			float[] result = new float[M];
			float[] w = new float[z.Length];

			for (int i = 0; i < z.Length; i++)
			{
				w[i] = (float)1.0;
			}

			for (int i = 1; i <= M - 1; i++)
			{
				int head = (i - 1) * k / (M - 1) + 1;
				int tail = i * k / (M - 1);
				float[] subZ = SubVector(z, head - 1, tail - 1);
				float[] subW = SubVector(w, head - 1, tail - 1);

				result[i - 1] = Transformations.R_sum(subZ, subW);
			}

			int h = k + 1;
			int t = z.Length;

			float[] sZ = SubVector(z, h - 1, t - 1);
			float[] sW = SubVector(w, h - 1, t - 1);
			result[M - 1] = Transformations.R_sum(sZ, sW);

			return result;
		}

		#endregion

		#region Public Overrides

		/// <summary>
		/// Evaluates a solution
		/// </summary>
		/// <param name="z">The solution to evaluate</param>
		/// <returns>float[] with the evaluation results</returns>
		public override float[] Evaluate(float[] z)
		{
			float[] y;

			y = Normalise(z);
			y = T1(y, k);
			y = T2(y, k, M);

			float[] result = new float[M];
			float[] x = Calculate_x(y);
			for (int m = 1; m <= M; m++)
			{
				result[m - 1] = D * x[M - 1] + S[m - 1] * Shapes.Concave(x, m);
			}

			return result;
		}

		/// <summary>
		/// Evaluates a solution 
		/// </summary>
		/// <param name="solution">The solution to evaluate</param>
		public override void Evaluate(Solution solution)
		{
			float[] variables = GetVariableValues(solution.Variable);

			float[] sol = Evaluate(variables);

			for (int i = 0; i < sol.Length; i++)
			{
				solution.Objective[i] = sol[i];
			}
		}
		#endregion
	}
}
