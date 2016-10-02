## TL/DR;
This project is split up into independent contexts that communicate via message passing.
In a nutshell, every incoming request/message is stuck unto a request queue, while a single message processor continuously dequeues, processes, and commits each request in a log and publishes any relevant events(messages) for any other interested services, all in a transcation.

For more information, see http://davidolubajo.com/posts/2016/09/29/log-based-single-processing-service.html