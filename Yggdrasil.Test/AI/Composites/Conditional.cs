﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Xunit;
using Yggrasil.Ai;
using Yggrasil.Ai.Composites;

namespace Yggdrasil.Test.AI.Composites
{
	public class ConditionalTests
	{
		[Fact]
		public void Conditional()
		{
			var state = new State();
			var test = 0;

			var sequence = new Sequence(
				new Conditional((_) => test < 50)
			);

			for (; test < 50; ++test)
			{
				Assert.Equal(RoutineStatus.Success, sequence.Act(state));
				state.Reset();
			}

			test++;
			Assert.Equal(RoutineStatus.Failure, sequence.Act(state));
			state.Reset();

			Assert.Equal(RoutineStatus.Failure, sequence.Act(state));
			state.Reset();

			test = 1;
			Assert.Equal(RoutineStatus.Success, sequence.Act(state));
			state.Reset();
		}
	}
}