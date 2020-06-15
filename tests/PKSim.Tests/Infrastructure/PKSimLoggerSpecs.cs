﻿using OSPSuite.BDDHelper;
using PKSim.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using DevExpress.Utils.VisualEffects;
using Microsoft.Extensions.Logging;
using DevExpress.Utils.Extensions;
using FluentNHibernate.Utils;
using FakeItEasy.Configuration;

namespace PKSim.Infrastructure
{
  public abstract class concern_for_PKSimLogger : ContextSpecification<PKSimLogger>
  {
    protected override void Context()
    {
      sut = new PKSimLogger();
    }
  }

  public class When_the_pk_sim_logger_is_configured_with_one_function : concern_for_PKSimLogger
  {
    private Func<ILoggingBuilder, ILoggingBuilder> _f1;

    protected override void Context()
    {
      base.Context();
      _f1 = A.Fake<Func<ILoggingBuilder, ILoggingBuilder>>();
    }

    protected override void Because()
    {
      sut.AddLoggingBuilderConfiguration(_f1);
    }

    [Observation]
    public void should_invoke_new_action()
    {
      A.CallTo(() => _f1.Invoke(A<ILoggingBuilder>.Ignored)).MustHaveHappened();
    }
  }


  public class When_the_pk_sim_logger_is_configured_with_some_function : concern_for_PKSimLogger
  {
    
    [Observation]
    public void should_invoke_all_actions_in_order()
    {
      var iterations = 10;
      var functions = new List<Func<ILoggingBuilder, ILoggingBuilder>>();
      for (var i = 0; i < iterations; i++)
      {
        functions.Add(A.Fake<Func<ILoggingBuilder, ILoggingBuilder>>());
      }

      for (var i = 0; i < iterations; i++)
      {
        sut.AddLoggingBuilderConfiguration(functions[i]);
        IOrderableCallAssertion check = A.CallTo(() => functions[0].Invoke(A<ILoggingBuilder>.Ignored)).MustHaveHappened();
        for (var j = 1; j < i; j++)
        {
          check = check.Then(A.CallTo(() => functions[j].Invoke(A<ILoggingBuilder>.Ignored)).MustHaveHappened());
        }
        for (var j = i + 1; j < iterations; j++)
        {
          A.CallTo(() => functions[j].Invoke(A<ILoggingBuilder>.Ignored)).MustNotHaveHappened();
        }
        for (var j = 0; j < i; j++)
        {
          Fake.ClearConfiguration(functions[i]);
        }
      }
    }
  }
}