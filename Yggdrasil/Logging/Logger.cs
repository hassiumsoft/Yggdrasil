﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Yggdrasil.Logging
{
	public sealed class Logger
	{
		private Regex _codeRegex = new Regex(@"\^[a-z]+[0-9]*;", RegexOptions.Compiled);

		private static Dictionary<string, Logger> _loggers;
		private List<LoggerTarget> _targets;

		/// <summary>
		/// Name of the logger.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Creates new logger.
		/// </summary>
		/// <param name="name"></param>
		private Logger(string name)
		{
			_targets = new List<LoggerTarget>();
			this.Name = name;
		}

		/// <summary>
		/// Initializes logger collection.
		/// </summary>
		static Logger()
		{
			_loggers = new Dictionary<string, Logger>();
		}

		/// <summary>
		/// Creates new logger.
		/// </summary>
		/// <returns></returns>
		public static Logger Get()
		{
			return Get(Assembly.GetCallingAssembly().GetName().Name);
		}

		/// <summary>
		/// Creates new named logger.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Logger Get(string name)
		{
			if (_loggers.ContainsKey(name))
				return _loggers[name];

			return (_loggers[name] = new Logger(name));
		}

		/// <summary>
		/// Creates new named logger.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Logger Get(object name)
		{
			return Get(name.ToString());
		}

		/// <summary>
		/// Creates new named logger.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Logger Get<T>()
		{
			return Get(typeof(T).ToString());
		}

		/// <summary>
		/// Adds target to this logger.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public Logger AddTarget(LoggerTarget target)
		{
			target.Logger = this;
			lock (_targets)
				_targets.Add(target);

			return this;
		}

		/// <summary>
		/// Returns list of all targets.
		/// </summary>
		/// <returns></returns>
		public LoggerTarget[] GetTargets()
		{
			lock (_targets)
				return _targets.ToArray();
		}

		/// <summary>
		/// Logs information.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Info(string format, params object[] args)
		{
			this.WriteLine(LogLevel.Info, string.Format(format, args));
		}

		/// <summary>
		/// Logs information.
		/// </summary>
		/// <remarks>
		/// Uses obj's ToString method.
		/// </remarks>
		/// <param name="obj"></param>
		public void Info(object obj) { this.Info(obj.ToString()); }

		/// <summary>
		/// Logs warning.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Warning(string format, params object[] args)
		{
			this.WriteLine(LogLevel.Warning, string.Format(format, args));
		}

		/// <summary>
		/// Logs warning.
		/// </summary>
		/// <remarks>
		/// Uses obj's ToString method.
		/// </remarks>
		/// <param name="obj"></param>
		public void Warning(object obj) { this.Warning(obj.ToString()); }

		/// <summary>
		/// Logs error.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Error(string format, params object[] args)
		{
			WriteLine(LogLevel.Error, string.Format(format, args));
		}

		/// <summary>
		/// Logs error.
		/// </summary>
		/// <remarks>
		/// Uses obj's ToString method.
		/// </remarks>
		/// <param name="obj"></param>
		public void Error(object obj) { this.Error(obj.ToString()); }

		/// <summary>
		/// Logs status message.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Status(string format, params object[] args)
		{
			this.WriteLine(LogLevel.Status, string.Format(format, args));
		}

		/// <summary>
		/// Logs status message.
		/// </summary>
		/// <remarks>
		/// Uses obj's ToString method.
		/// </remarks>
		/// <param name="obj"></param>
		public void Status(object obj) { this.Status(obj.ToString()); }

		/// <summary>
		/// Logs debug message.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Debug(string format, params object[] args)
		{
			this.WriteLine(LogLevel.Debug, string.Format(format, args));
		}

		/// <summary>
		/// Logs debug message.
		/// </summary>
		/// <remarks>
		/// Uses obj's ToString method.
		/// </remarks>
		/// <param name="obj"></param>
		public void Debug(object obj) { this.Debug(obj.ToString()); }

		/// <summary>
		/// Writes message to log.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void Write(LogLevel level, string message)
		{
			var dt = DateTime.Now;

			lock (_targets)
			{
				foreach (var target in _targets)
				{
					if (target.Filtered(level))
						continue;

					var messageRaw = string.Format(target.GetFormat(level), level, message);
					var messageClean = _codeRegex.Replace(messageRaw, "");

					target.Write(level, message, messageRaw, messageClean);
				}
			}
		}

		/// <summary>
		/// Writes message + line break to log.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="message"></param>
		public void WriteLine(LogLevel level, string message)
		{
			this.Write(level, message + Environment.NewLine);
		}
	}
}
