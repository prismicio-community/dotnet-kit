using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Threading.Tasks;
using prismic.extensions;

namespace prismic.csharp.tests
{
	[TestFixture ()]
	public class ApiTest
	{
		[Test ()]
		public void GetPrivateApiWithoutAuthorizationTokenShouldThrow()
		{
			var url = "https://private-test.prismic.io/api";

			ExpectInnerException<Api.AuthorizationNeeded>( 
				() => prismic.extensions.Api.Get (url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {}).Wait(),
				e => 
				"https://private-test.prismic.io/auth" == e.Data0);
		}

		[Test ()]
		public void GetPrivateApiWithInvalidTokenShouldThrow()
		{
			var url = "https://private-test.prismic.io/api";

			ExpectInnerException<Api.InvalidToken>( 
				() => prismic.extensions.Api.Get ("dummy-token", url, new prismic.Infra.NoCache<prismic.Api.Response>(), (l, m) => {}).Wait(),
				e => 
				"https://private-test.prismic.io/auth" == e.Data0);
		}

		private void ExpectInnerException<ExT>(Action action, Func<ExT, bool> exceptionPredicate) where ExT : Exception
		{
			try {
				ThrowInner(action);
				Assert.Fail("expected exception was not raised");
			} catch (ExT ex) {
				exceptionPredicate (ex);
			} catch (Exception ex) {
				Assert.Fail(String.Format("unexpected type of exception happened: {0} {1}", ex.GetType().Name, ex.Message));
			}
		}

		private void ThrowInner(Action action) 
		{
			try {
				action();
			} catch (AggregateException ex) {
				throw ex.InnerException;
			}
		}

	}
}

