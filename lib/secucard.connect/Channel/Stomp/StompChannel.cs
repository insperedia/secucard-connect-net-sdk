using System;
using Secucard.Connect.Channel;
using Secucard.Model;

public class StompChannel : StompChannelBase,IChannel {

  public StompChannel(String id, Configuration cfg) {
    base(id, cfg);
  }

  //synchronized
  public  override void Open(final Callback callback)  {
    if (connected) {
      onCompleted(callback, null);
      return;
    }

    try {
      String login = configuration.getUserId();
      String pwd = configuration.getPassword();
      if (login == null || pwd == null) {
        String token = authProvider.getToken().getAccessToken();
        login = pwd = token;
      }
      stompSupport.open(login, pwd);
    } catch (Exception e) {
      if (callback == null) {
        throw e;
      } else {
        onFailed(callback, e);
        return;
      }
    }

    if (callback == null) {
      awaitConnection(null);
    } else {
      new AsyncExecution() {
        @Override
        protected void run() {
          awaitConnection(callback);
        }
      }.start();
    }
  }

  @Override
  public synchronized void close(Callback callback) {
    if (connected) {
      try {
        String id = stompSupport.close();
        awaitReceipt(id, callback);
      } catch (Exception e) {
        if (callback == null) {
          throw e;
        }
        onFailed(callback, e);
      }
    }
  }

  public String ping() {
    Result result = sendMessage(new StandardDestination(null) {
      @Override
      public String toString() {
        return getBasicDestination() + "ping";
      }
    }, null, new MessageTypeRef(Result.class), null, false);

    return result == null ? null : result.getResult();
  }

  public T GetObject(T type, String objectId, final Func<T> callback) {
    return sendMessage(new StandardDestination(StandardDestination.GET, type), new Message<T>(objectId),
        new MessageTypeRef(type), callback, true);
  }

  @Override
  public <T> ObjectList<T> findObjects(final Class<T> type, QueryParams queryParams, Callback<ObjectList<T>> callback) {
    Message message = new Message();
    message.setQuery(queryParams);

    StatusHandler statusHandler = new StatusHandler() {
      @Override
      public boolean hasError(Message message) {
        // treat not found as ok here, no matches for query
        return !(STATUS_OK.equalsIgnoreCase(message.getStatus())
            || "ProductNotFoundException".equalsIgnoreCase(message.getError()));
      }
    };

    return sendMessage(new StandardDestination(StandardDestination.GET, type), message, new MessageListTypeRef(type),
        statusHandler, callback, true);
  }

  @Override
  public <T> T createObject(T object, Callback<T> callback) {
    return sendMessage(new StandardDestination(StandardDestination.ADD, object.getClass()),
        new Message<>(object), new MessageTypeRef(object.getClass()), callback, true);
  }

  @Override
  public <T extends SecuObject> T updateObject(T object, Callback<T> callback) {
    return sendMessage(new StandardDestination(StandardDestination.UPDATE, object.getClass()),
        new Message<>(object.getId(), object), new MessageTypeRef(object.getClass()), callback, true);
  }

  @Override
  public <T> T updateObject(Class product, String objectId, String action, String actionArg, Object arg,
                            Class<T> returnType, Callback<T> callback) {
    return sendMessage(new StandardDestination(StandardDestination.UPDATE, product, action),
        new Message<>(objectId, actionArg, arg), new MessageTypeRef(returnType), callback, true);
  }

  @Override
  public void deleteObject(Class type, String objectId, Callback callback) {
    sendMessage(new StandardDestination(StandardDestination.DELETE, type), new Message<>(objectId),
        new MessageTypeRef(type), callback, true);
  }

  @Override
  public void deleteObject(Class product, String objectId, String action, String actionArg, Callback<?> callback) {
    sendMessage(new StandardDestination(StandardDestination.DELETE, product, action), new Message<>(objectId, actionArg),
        new MessageTypeRef(product), callback, true);
  }

  @Override
  public <T> T execute(String appId, String action, Object arg, Class<T> returnType, Callback<T> callback) {
    return sendMessage(new AppDestination(appId, action), arg, new MessageTypeRef(returnType), callback, true);
  }

  public T Execute(Class product, String objectId, String action, String actionArg, Object arg, T returnType, func<T> callback) {
    return sendMessage(new StandardDestination(StandardDestination.EXEC, product, action),
        new Message<>(objectId, actionArg, arg), new MessageTypeRef(returnType), callback, true);
  }
}