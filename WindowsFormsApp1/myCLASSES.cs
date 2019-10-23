using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 二进制编码操作类
/// </summary>
public class GeneCoding
{
	private int _bitCount;
	public int BitCount { get => _bitCount; }

	/// <summary>
	/// 构造函数
	/// </summary>
	/// <param name="bits">二进制编码的位数</param>
	public GeneCoding(int bits)
	{
		_bitCount = bits;
	}

	/// <summary>
	/// 二进制编码操作，输出bool数组
	/// </summary>
	/// <param name="input">需要编码的数</param>
	/// <param name="input_Max">数字范围最大值</param>
	/// <param name="input_Min">数字范围最小值</param>
	/// <param name="gene">输出bool数组</param>
	/// <returns>是否成功编码</returns>
	public bool Encode(double input, double input_Max, double input_Min, out bool[] gene)
	{
		string tempGene;

		gene = new bool[_bitCount];
		int encodedNum = (int)(((input - input_Min) * (Math.Pow(2.0, 20.0) - 1)) / (input_Max - input_Min));

		tempGene = Convert.ToString(encodedNum, 2);
		if (tempGene.Length != _bitCount)//如果不足20说明前面需要补0
		{
			int addNumber = _bitCount - tempGene.Length;//需要补几个0
			List<char> temp = tempGene.ToList<char>();
			for (int i = 0; i < addNumber; i++)
			{
				temp.Insert(0, '0');
			}

			string _tempString = new string(temp.ToArray());
			tempGene = _tempString;
		}

		for (int i = 0; i < tempGene.Length; i++)
		{
			if (tempGene[i] == '1')
			{
				gene[i] = true;
			}
			else if (tempGene[i] == '0')
			{
				gene[i] = false;
			}
			else
			{
				gene = null;
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// 二进制编码的解码操作，输出double
	/// </summary>
	/// <param name="input">输入的布尔数组</param>
	/// <param name="input_Max">数字范围最大值</param>
	/// <param name="input_Min">数字范围最小值</param>
	/// <param name="result">输出double结果</param>
	/// <returns>是否成功解码</returns>
	public bool Decode(bool[] input, double input_Max, double input_Min, out double result)
	{
		result = 0;
		int tempResult = 0;
		int pow = 0;
		try
		{
			for (int i = input.Length - 1; i >= 0; i--)
			{
				if (input[i] == true)//1
				{
					tempResult += (1 * (int)Math.Pow(2, pow));
				}
				pow++;
			}
		}
		catch
		{
			result = 0;
			return false;
		}

		result = (input_Max - input_Min) / (Math.Pow(2, _bitCount) - 1) * tempResult + input_Min;
		return true;
	}
}

/// <summary>
/// 种群类
/// </summary>
public class Population
{
	/// <summary>
	/// 数值
	/// </summary>
	private double _value;
	public double Value { get => _value; set => _value = value; }

	/// <summary>
	/// 数值最大值
	/// </summary>
	private double _maxValue;
	public double MaxValue { get => _maxValue; }

	/// <summary>
	/// 数值最小值
	/// </summary>
	private double _minValue;
	public double MinValue { get => _minValue; }

	/// <summary>
	/// 基因
	/// </summary>
	private bool[] _gene;
	public bool[] Gene { get => _gene; set => _gene = value; }

	/// <summary>
	/// 适应度
	/// </summary>
	private double _fitness;
	public double Fitness { get => _fitness; }

	/// <summary>
	/// 构造函数
	/// </summary>
	/// <param name="inputMax">设定最大值</param>
	/// <param name="inputMin">设定最小值</param>
	public Population(double inputMax, double inputMin)
	{
		_maxValue = inputMax;
		_minValue = inputMin;
	}
	/// <summary>
	/// 无参构造函数
	/// </summary>
	public Population()
	{
		_maxValue = 0;
		_minValue = 0;
		_value = 0;
	}
	/// <summary>
	/// 初始化取值范围
	/// </summary>
	/// <param name="inputMax">最大值</param>
	/// <param name="inputMin">最小值</param>
	public void SetMaxMin(double inputMax, double inputMin)
	{
		_maxValue = inputMax;
		_minValue = inputMin;
	}

	/// <summary>
	/// 适应度计算函数
	/// </summary>
	/// <returns>是否成功计算</returns>
	public bool CalFitness()
	{
		// y=x.*sin(10.*pi.*x)+2
		_fitness = _value * Math.Sin(10 * Math.PI * _value) + 2;//适应度计算函数
		return true;
	}

	/// <summary>
	/// 计算适应度
	/// </summary>
	/// <param name="x">输入自变量</param>
	/// <returns>计算结果</returns>
	public double CalFitness(double x)
	{
		return (x * Math.Sin(10 * Math.PI * x) + 2);
	}

	/// <summary>
	/// 复制函数
	/// </summary>
	/// <returns>拷贝</returns>
	public Population Copy()
	{
		Population temp = new Population(_maxValue,_minValue);
		temp.Value = _value;
		//temp.MaxValue = _maxValue;
		//temp.MinValue = _minValue;
		temp.CalFitness();
		temp.Gene = Gene;

		return temp;
	}
}

/// <summary>
/// 基因操作类
/// </summary>
public class GeneOperation
{
	private Random rad;
	private GeneCoding _geneCoder;//基因编码操作子

	double _crossRate;//交叉概率
	double _mutateRate;//突变概率
	int _codeBits;//编码位数

	public double CrossRate { get => _crossRate; }
	public double MutateRate { get => _mutateRate; }
	public int CodeBits { get => _codeBits; }

	/// <summary>
	/// 构造函数,初始化交叉概率和突变概率
	/// </summary>
	/// <param name="CrossRate">初始化交叉概率,0~1</param>
	/// <param name="MutateRate">初始化突变概率,0~1</param>
	/// <param name="CodeBits">编码位数，必须大于0</param>
	public GeneOperation(double CrossRate, double MutateRate, int CodeBits)
	{
		_crossRate = CrossRate;
		_mutateRate = MutateRate;
		_codeBits = CodeBits;
		rad = new Random();

		_geneCoder = new GeneCoding(_codeBits);
	}

	/// <summary>
	/// 初始化种群
	/// </summary>
	/// <param name="maxValue">种群数值最大值</param>
	/// <param name="minValue">种群数值最小值</param>
	/// <param name="unitNumber">种群数量</param>
	/// <returns>初始化完成的结果种群，Population[]</returns>
	public Population[] InitPopulation(double maxValue, double minValue, int unitNumber)
	{
		Population[] tempPop = new Population[unitNumber];//初始化种群

		for (int i = 0; i < tempPop.Length; i++)//对每一个成员
		{
			tempPop[i] = new Population(maxValue, minValue);
			tempPop[i].Value = minValue + (rad.NextDouble() * (maxValue - minValue));//随机初始化数值
			tempPop[i].CalFitness();//计算初始值的适应度
			_geneCoder.Encode(tempPop[i].Value, maxValue, minValue, out bool[] outGene);//编码基因
			tempPop[i].Gene = outGene;//写入基因
		}

		return tempPop;//返回结果
	}

	/// <summary>
	/// 轮盘赌选择，对适应度标准化后执行轮盘赌选择
	/// </summary>
	/// <param name="input_Unit">输入种群数组</param>
	/// <returns>种群索引</returns>
	private int GetChromoRoulette(Population[] input_Unit)
	{
		//Console.WriteLine("======ChromoRouletteCheck======");
		//查找最低适应度
		double worstFitness = input_Unit[0].Fitness;
		foreach (Population e in input_Unit)
		{
			if (e.Fitness < worstFitness)//如果比最低适应度还小
			{
				worstFitness = e.Fitness;//保存最低适应度
			}
		}

		//标准化适应度并进行轮盘赌选择
		double totalFitness = 0;
		foreach (Population e in input_Unit)
		{
			totalFitness += (e.Fitness - worstFitness);//加适应度减最低适应度，标准化

			//Console.Write("{0}\t", totalFitness);
		}
		double Slice = (rad.NextDouble()) * totalFitness;

		//Console.WriteLine("\nSlice:"+Slice.ToString());

		totalFitness = 0;
		int theChosenIndex = 0;
		foreach (Population e in input_Unit)
		{
			totalFitness += (e.Fitness - worstFitness);//加适应度减最低适应度，标准化
			if (totalFitness >= Slice)
			{
				break;
			}
			else
			{
				theChosenIndex++;
			}
		}

		//Console.WriteLine("Chosen:{0}",theChosenIndex);

		return theChosenIndex;
	}

	/// <summary>
	/// 选择最佳适应度
	/// </summary>
	/// <param name="input_Unit">输入种群</param>
	/// <param name="bestFitness">最佳适应度</param>
	/// <returns>最佳适应度个体索引</returns>
	public int SelectBest(Population[] input_Unit, out double bestFitness)
	{
		bestFitness = 0;
		int bestIndex = 0;
		for (int i = 0; i < input_Unit.Length; i++)
		{
			if (input_Unit[i].Fitness > bestFitness)
			{
				bestFitness = input_Unit[i].Fitness;
				bestIndex = i;
			}
		}
		return bestIndex;
	}

	/// <summary>
	/// 交叉操作
	/// </summary>
	/// <param name="input_Unit">输入父母个体</param>
	/// <returns>是否操作成功</returns>
	private bool Cross(ref Population dad_Unit, ref Population mum_Unit)
	{
		int crossStartIndex = rad.Next(_codeBits - 2);
		int crossEndIndex = rad.Next(crossStartIndex, _codeBits - 1);

		//获取表
		List<bool> dad_UnitGene = dad_Unit.Gene.ToList<bool>();
		List<bool> mum_UnitGene = mum_Unit.Gene.ToList<bool>();

		List<bool> geneSlice = dad_UnitGene.GetRange(crossStartIndex, crossEndIndex - crossStartIndex + 1);//获取需要交叉的基因片段

		//交换基因片段
		dad_UnitGene.RemoveRange(crossStartIndex, crossEndIndex - crossStartIndex + 1);
		dad_UnitGene.InsertRange(crossStartIndex, mum_UnitGene.GetRange(crossStartIndex, crossEndIndex - crossStartIndex + 1));

		mum_UnitGene.RemoveRange(crossStartIndex, crossEndIndex - crossStartIndex + 1);
		mum_UnitGene.InsertRange(crossStartIndex, geneSlice);

		//基因片段写入Population中
		dad_Unit.Gene = dad_UnitGene.ToArray();
		mum_Unit.Gene = mum_UnitGene.ToArray();

		return true;
	}

	/// <summary>
	/// 变异操作
	/// </summary>
	/// <param name="input_Unit">输入要执行操作的个体</param>
	/// <returns>是否操作成功</returns>
	private bool Mutate(ref Population unit)
	{
		int mutateIndex = rad.Next(_codeBits - 1);//随机选择要变异的基因
		List<bool> tempGene = unit.Gene.ToList<bool>();//复制表

		//变异操作
		tempGene[mutateIndex] = !tempGene[mutateIndex];//取反

		//写入Population
		unit.Gene = tempGene.ToArray();

		return true;
	}

	/// <summary>
	/// 繁衍操作
	/// </summary>
	/// <param name="input_Unit">父代种群</param>
	/// <returns>下一代种群数组</returns>
	public Population[] NextGeneration(Population[] input_Unit)
	{
		Population[] tempPop = new Population[input_Unit.Length];

		Population dad_Unit;
		Population mum_Unit;

		for (int i = 0; i < tempPop.Length; i++)
		{
			#region 轮盘赌选择父代
			//step1 轮盘赌选择父代
			dad_Unit = input_Unit[GetChromoRoulette(input_Unit)].Copy();
			mum_Unit = input_Unit[GetChromoRoulette(input_Unit)].Copy();
			#endregion

			#region 判断是否需要交叉
			//step2 判断是否需要交叉
			if (rad.NextDouble() < _crossRate)//需要交叉
			{
				if (Cross(ref dad_Unit, ref mum_Unit) != true)//如果没有成功执行
				{
					//添加处理代码
				}
			}
			#endregion

			#region 判断是否需要变异
			//step3 判断是否需要变异
			if (rad.NextDouble() < _mutateRate)
			{
				if (Mutate(ref dad_Unit) != true)//如果没有成功执行
				{
					//添加处理代码
				}
			}
			if (rad.NextDouble() < _mutateRate)
			{
				if (Mutate(ref mum_Unit) != true)//如果没有成功执行
				{
					//添加处理代码
				}
			}
			#endregion

			#region 繁殖
			//step4 繁殖
			if (rad.NextDouble() >= 0.5)
			{
				if (i < tempPop.Length)
				{
					tempPop[i] = dad_Unit.Copy();
					i++;
				}
				if (i < tempPop.Length)
				{
					tempPop[i] = mum_Unit.Copy();
				}
			}
			else
			{
				if (i < tempPop.Length)
				{
					tempPop[i] = mum_Unit.Copy();
					i++;
				}
				if (i < tempPop.Length)
				{
					tempPop[i] = dad_Unit.Copy();
				}
			}
			#endregion

			//#region 精英原则
			//int bestIndex = SelectBest(input_Unit, out double noUseBest);
			//tempPop[rad.Next(rad.Next(tempPop.Length))] = input_Unit[bestIndex];
			//#endregion
		}

		#region 更新适应度和数值
		//step5 更新适应度和数值
		for (int i = 0; i < tempPop.Length; i++)
		{
			_geneCoder.Decode(tempPop[i].Gene, tempPop[i].MaxValue, tempPop[i].MinValue, out double result);//解码基因
			tempPop[i].Value = result;
			tempPop[i].CalFitness();//计算适应度
		}
		#endregion

		#region 爬山
		for (int i = 0; i < tempPop.Length; i++)
		{
			double delta = rad.NextDouble() * 0.001;
			double orignValue = tempPop[i].Value;
			double leftValue = tempPop[i].Value - delta;//左走
			double rightValue = tempPop[i].Value + delta;//右走
			double leftFitness = 0;
			double rightFitness = 0;
			double orignFitness = tempPop[i].Fitness;

			//向左
			if (leftValue <= tempPop[i].MinValue)
			{
				leftValue = tempPop[i].MinValue;
			}
			tempPop[i].Value = leftValue;
			tempPop[i].CalFitness();
			leftFitness = tempPop[i].Fitness;

			//向右
			if (rightValue >= tempPop[i].MaxValue)
			{
				rightValue = tempPop[i].MaxValue;
			}
			tempPop[i].Value = rightValue;
			tempPop[i].CalFitness();
			rightFitness = tempPop[i].Fitness;

			if (rightFitness > leftFitness)//右>左
			{
				if (rightFitness > orignFitness)
				{
					//向右
					tempPop[i].Value = rightValue;
				}
				else
				{
					//不动
					tempPop[i].Value = orignValue;
				}
			}
			else//右<左
			{
				if (leftFitness > orignFitness)
				{
					//向左
					tempPop[i].Value = leftValue;
				}
				else
				{
					//不动
					tempPop[i].Value = orignValue;
				}
			}

			//if (rightFitness > leftFitness)//如果向右优于向左
			//{
			//	tempPop[i].Value = rightValue;
			//}
			//else
			//{
			//	tempPop[i].Value = leftValue;//向左爬山
			//}

			//更新适应度
			tempPop[i].CalFitness();
			//更新基因
			_geneCoder.Encode(tempPop[i].Value, tempPop[i].MaxValue, tempPop[i].MinValue, out bool[] tempGene);
			tempPop[i].Gene = tempGene;
		}
		#endregion

		return tempPop;//返回子代
	}
}