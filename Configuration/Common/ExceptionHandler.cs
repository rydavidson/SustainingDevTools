using System;

namespace rydavidson.Accela.Configuration.Common
{
    public abstract class ExceptionHandler
    {
        private Logger logger;

        public void HandleException(Exception _e, string _message = "")
        {
            HandleException(_e, logger, _message);
        }

        public void HandleException(Exception _e, Logger _logger, string _message = "")
        {
            LogException(_e, _logger, _message);
        }

        private void LogException(Exception _e, string _message)
        {
            LogException(_e, logger, _message);
        }

        private void LogException(Exception _e, Logger _logger, string _message = "")
        {
            SetLogger(_logger);

#if DEBUG
            Console.WriteLine(_message);
            Console.WriteLine(_e.Message);
            Console.WriteLine(_e.StackTrace);
#endif
            if (!string.IsNullOrWhiteSpace(_message))
                logger.Error(_message);
            if (!string.IsNullOrWhiteSpace(_e.Message))
                logger.Error(_e.Message);
            if (!string.IsNullOrWhiteSpace(_e.StackTrace))
                logger.Error(_e.StackTrace);
        }

        public void SetLogger(Logger _logger)
        {
            logger = _logger;
        }

    }
}
