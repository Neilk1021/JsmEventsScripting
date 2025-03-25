using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace JSM.JSME
{
    public class JsmeCommandDecorator : JsmeCommand
    {
        private readonly JsmeCommand _command;
        public JsmeCommandDecorator(JsmeCommand command) : base(command.GetRunner())
        {
            _command = command;
        }

        public override void Execute()
        {
            _command.Execute();
        }
    }

    public class JsmeAsyncDecorator : JsmeCommandDecorator
    {
        private readonly JsmeCommand _command;
        public JsmeAsyncDecorator(JsmeCommand command) : base (command)
        {
            _command = command;
        }

        public override void Execute()
        {
            Task.Run(() => _command.Execute());
        }
    }
    
    public class JsmeAwaitCommand : JsmeCommand
    {
        private readonly Func<bool> _condition; 
        public JsmeAwaitCommand(JsmeRunner runner, Func<bool> condition) : base (runner)
        {
            _condition = condition;
        }

        public override void Execute()
        {
            Runner.StartAwait();
            Runner.StartCoroutine(WaitAndCheckCondition());
        }
        
        private IEnumerator WaitAndCheckCondition()
        {
            while (!_condition())
            {
                yield return null;   
            }
            Runner.EndAwait();
        }
    }

    
    /// <summary>
    /// Adds a conditional check to a JSME command.
    /// </summary>
    public class JsmeConditionalDecorator : JsmeCommandDecorator
    {
        private readonly JsmeCommand _command;
        private readonly Func<bool> _condition; 

        /// <summary>
        /// Decorates a JSME command with a conditional check. 
        /// </summary>
        /// <param name="command">Original command object.</param>
        /// <param name="condition">Condition that should be met</param>
        public JsmeConditionalDecorator(JsmeCommand command, Func<bool> condition) : base (command)
        {
            _command = command;
            _condition = condition;
        }

        public override void Execute()
        {
            if (_condition())
            {
                _command.Execute();   
            }
        }
    }
    
    /// <summary>
    /// Adds a conditional check to a JSME command.
    /// </summary>
    public class JsmeOnCompleteDecorator : JsmeCommandDecorator
    {
        private readonly JsmeCommand _command;
        private readonly string _commandLookup;
        
        /// <summary>
        /// Decorates a JSME command with a onComplete check. 
        /// </summary>
        /// <param name="command"></param>
        /// Original command object.
        /// <param name="commandLookup">
        /// Label to search for in the runners cache.
        /// </param>
        public JsmeOnCompleteDecorator(JsmeCommand command, string commandLookup) : base (command)
        {
            _command = command;
            _commandLookup = commandLookup;
        }

        public override void Execute()
        {
            _command.Execute();
            Runner.GetCache().GetLookup(_commandLookup).Execute();
        }
    }
    
    
    /// <summary>
    /// Command to print something to the DEBUG line. 
    /// </summary>
    public class JsmePrintCommand : JsmeCommand
    {
        private readonly string _contents;
        
        public JsmePrintCommand(JsmeRunner runner, string contents) : base(runner)
        {
            _contents = contents;
            Runner = runner;
        }

        public override void Execute()
        {
            Debug.Log(_contents);
        }
    }
    
    public class JsmeRunCommand : JsmeCommand
    {
        private readonly Delegate _function;
        private readonly Func<object>[] _vals;
        
        public JsmeRunCommand(JsmeRunner runner, Delegate function, params Func<object>[] vals) : base(runner)
        {
            _function = function;
            _vals = vals;
        }

        public override void Execute()
        {
            object[] args = _vals.Select(x => x()).ToArray();
            
            _function.DynamicInvoke(args);
        }
    }


    public class JsmeAddCommand : JsmeCommand
    {
        private readonly string _key;
        private readonly float _val;
        public JsmeAddCommand(JsmeRunner runner, string key, float val) : base(runner)
        {
            _key = key;
            _val = val;
        }

        public override void Execute()
        {
            var cache = Runner.GetCache();
            cache.UpdateVariable(_key, cache.GetVariable<float>(_key) + _val);
        }
    }
    
    public class JsmeSubCommand : JsmeCommand
    {
        private readonly string _key;
        private readonly float _val;
        public JsmeSubCommand(JsmeRunner runner, string key, float val) : base(runner)
        {
            _key = key;
            _val = val;
        }

        public override void Execute()
        {
            var cache = Runner.GetCache();
            cache.UpdateVariable(_key, cache.GetVariable<float>(_key) - _val);
        }
    }

    public class JsmeMultiplyCommand : JsmeCommand
    {
        private readonly string _key;
        private readonly float _val;
        public JsmeMultiplyCommand(JsmeRunner runner, string key, float val) : base(runner)
        {
            _key = key;
            _val = val;
        }

        public override void Execute()
        {
            var cache = Runner.GetCache();
            cache.UpdateVariable(_key, cache.GetVariable<float>(_key) * _val);
        }
    }

    
    public class JsmeDivideCommand : JsmeCommand
    {
        private readonly string _key;
        private readonly float _val;
        public JsmeDivideCommand(JsmeRunner runner, string key, float val) : base(runner)
        {
            _key = key;
            _val = val;
        }

        public override void Execute()
        {
            var cache = Runner.GetCache();
            cache.UpdateVariable(_key, cache.GetVariable<float>(_key) / _val);
        }
    }
    
    public class JsmeSetCommand : JsmeCommand
    {
        private readonly string _key;
        private readonly object _val;
        public JsmeSetCommand(JsmeRunner runner, string key, object val) : base(runner)
        {
            _key = key;
            _val = val;
        }

        public override void Execute()
        {
            var cache = Runner.GetCache();
            cache.UpdateVariable(_key, _val);
        }
    }

    

    
    public abstract class JsmeCommand
    {
        protected JsmeRunner Runner = null;
        
        protected JsmeCommand(JsmeRunner runner)
        {
            Runner = runner;
        }

        public JsmeRunner GetRunner()
        {
            return Runner;
        }
        
        public virtual void Execute()
        {
            throw new NotImplementedException("Execute not written");
        }
    }
}

