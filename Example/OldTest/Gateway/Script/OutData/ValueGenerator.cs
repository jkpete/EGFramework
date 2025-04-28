
using System;

public class ValueGenerator
{
	int Value = 0;
	float ValueFloat = 0f;
	public int GenerateValue(){
		Value ++;
		if(Value>10){
			Value = 0;
		}
		return Value;
	}

	public float GenerateFValue(){
		ValueFloat += 0.1f;
		if(ValueFloat>10){
			ValueFloat = 0;
		}
		return ValueFloat;
	}
}
