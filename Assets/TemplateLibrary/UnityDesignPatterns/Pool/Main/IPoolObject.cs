using System;

public interface IPoolObject :
ICloneable
{
	string	Name		{ get; set; }

	bool	Used		{ get; set; }
	IPool	PullSource	{ get; set; }

	void	InitPoolObject();

	void	OnPop();

	void	BeforeRelease();
	void	AfterRelease();

	void	FreeObject();
	void	DestroyObject();
}