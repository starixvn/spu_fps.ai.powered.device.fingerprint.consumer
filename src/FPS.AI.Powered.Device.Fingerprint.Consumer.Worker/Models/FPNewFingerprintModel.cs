using Device.FingerPrint.Core.Services;
using Device.FingerPrint.Model;

namespace Device.FingerPrint.QueueConsumer.Models
{
    public class FPNewFingerprintModel
    {
        private readonly FPFingerprintAttributeModel _attributeModel;
        private readonly ICrypto crypto;

        public FPNewFingerprintModel(FPFingerprintAttributeModel attributeModel, ICrypto crypto)
        {
            _attributeModel = attributeModel;
            this.crypto = crypto;
        }   

        public FPFingerprintAttributeModel AttributeModel 
        {
            get { return _attributeModel; }
        }

        public string Code => Utilities.GetMD5HashFromObject(crypto, _attributeModel);       
    }

    public class FPOldFingerprintModel
    {
        private readonly FPOldFingerprintAttributeModel _attributeModel;
        private readonly ICrypto crypto;

        public FPOldFingerprintModel(FPOldFingerprintAttributeModel attributeModel, ICrypto crypto)
        {
            _attributeModel = attributeModel;
            this.crypto = crypto;
        }

        public string Code => Utilities.GetMD5HashFromObject(crypto, _attributeModel);
        
    }

    public class FPFingerprintExtraModel
    {
        private readonly FPFingerprintAttributeExtraModel _attributeModel;
        private readonly ICrypto crypto;

        public FPFingerprintExtraModel(FPFingerprintAttributeExtraModel attributeModel, ICrypto crypto)
        {
            _attributeModel = attributeModel;
            this.crypto = crypto;
        }

        public FPFingerprintAttributeExtraModel AttributeModel
        {
            get { return _attributeModel; }
        }

        public string Code => Utilities.GetMD5HashFromObject(crypto, _attributeModel);
        
    }
}
